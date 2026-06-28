using System.Text.Json.Serialization;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Enums;
using Market.Domain.Exceptions;

namespace Market.Api.Seeders
{
    public class PokemonProductSeeder
    {
        private readonly HttpClient _httpClient;
        private readonly IProductService _productService;
        private readonly IStoreService _storeService;
        private readonly ICategoryService _categoryService;
        private readonly IProductCategoryService _productCategoryService;
        private readonly IProductInformationService _productInformationService;
        private readonly IProductPhotoService _productPhotoService;
        private readonly IProductPriceService _productPriceService;
        private readonly IProductStockService _productStockService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PokemonProductSeeder> _logger;

        public PokemonProductSeeder(
            IHttpClientFactory httpClientFactory,
            IProductService productService,
            IStoreService storeService,
            ICategoryService categoryService,
            IProductCategoryService productCategoryService,
            IProductInformationService productInformationService,
            IProductPhotoService productPhotoService,
            IProductPriceService productPriceService,
            IProductStockService productStockService,
            IConfiguration configuration,
            ILogger<PokemonProductSeeder> logger)
        {
            _httpClient = httpClientFactory.CreateClient();
            _productService = productService;
            _storeService = storeService;
            _categoryService = categoryService;
            _productCategoryService = productCategoryService;
            _productInformationService = productInformationService;
            _productPhotoService = productPhotoService;
            _productPriceService = productPriceService;
            _productStockService = productStockService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SeedAsync(CancellationToken cancellationToken = default)
        {
            var options = _configuration
                .GetSection(PokemonSeederOptions.SectionName)
                .Get<PokemonSeederOptions>() ?? new PokemonSeederOptions();

            if (!options.Enabled)
                return;

            var storeId = await GetStoreIdAsync(options, cancellationToken);

            for (var id = options.StartId; id <= options.EndId; id++)
            {
                try
                {
                    var pokemon = await GetFromPokeApiAsync<PokemonApiResponse>(
                        $"https://pokeapi.co/api/v2/pokemon/{id}",
                        cancellationToken);

                    var species = await GetFromPokeApiAsync<PokemonSpeciesApiResponse>(
                        $"https://pokeapi.co/api/v2/pokemon-species/{id}",
                        cancellationToken);

                    var categories = await GetOrCreateTypeCategoriesAsync(pokemon);

                    var product = await _productService.AddAsync(new ProductCreateDto
                    {
                        StoreId = storeId,
                        Name = ToTitleCase(pokemon.Name),
                        Code = pokemon.Id,
                        Slug = pokemon.Name,
                        Summary = BuildSummary(pokemon, species),
                        Description = BuildDescription(pokemon, species)
                    });

                    foreach (var category in categories)
                    {
                        await _productCategoryService.AddAsync(new ProductCategoryCreateDto
                        {
                            ProductId = product.Id,
                            CategoryId = category.Category.Id,
                            Order = category.Order
                        });
                    }

                    await AddProductInformationsAsync(product.Id, pokemon);
                    await AddProductPhotosAsync(product.Id, pokemon);
                    await AddProductPriceAsync(product.Id, pokemon);
                    await AddProductStockAsync(product.Id, pokemon);
                }
                catch (BusinessException ex) when (ex.Message.Contains("code already exists", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Pokemon product {PokemonId} already exists. Skipping.", id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not seed pokemon product {PokemonId}.", id);
                }
            }
        }

        private async Task<Guid> GetStoreIdAsync(PokemonSeederOptions options, CancellationToken cancellationToken)
        {
            if (options.StoreId.HasValue)
                return options.StoreId.Value;

            var stores = await _storeService.GetPagedAsync(new PaginationRequestDto { PageIndex = 1, PageSize = 100 });
            var existingStore = stores.Items.FirstOrDefault(store =>
                string.Equals(store.Name, options.StoreName, StringComparison.OrdinalIgnoreCase));

            if (existingStore is not null)
                return existingStore.Id;

            var store = await _storeService.AddAsync(new StoreCreateDto
            {
                Name = options.StoreName,
                Description = "Store created by the Pokemon product seeder.",
                UserCreatedId = options.UserCreatedId
            });

            return store.Id;
        }

        private async Task AddProductInformationsAsync(Guid productId, PokemonApiResponse pokemon)
        {
            var order = 1;
            var informations = new List<ProductInformationCreateDto>
            {
                new ProductInformationCreateDto
                {
                    ProductId = productId,
                    Type = InformationTypeEnum.Id,
                    Label = "PokeAPI Id",
                    Value = pokemon.Id.ToString(),
                    Order = order++
                },
                new ProductInformationCreateDto
                {
                    ProductId = productId,
                    Type = InformationTypeEnum.Height,
                    Label = "Height",
                    Value = pokemon.Height.ToString(),
                    Order = order++
                },
                new ProductInformationCreateDto
                {
                    ProductId = productId,
                    Type = InformationTypeEnum.Weight,
                    Label = "Weight",
                    Value = pokemon.Weight.ToString(),
                    Order = order++
                },
                new ProductInformationCreateDto
                {
                    ProductId = productId,
                    Type = InformationTypeEnum.BaseExperience,
                    Label = "Base Experience",
                    Value = pokemon.BaseExperience.ToString(),
                    Order = order++
                }
            };

            foreach (var type in pokemon.Types.OrderBy(type => type.Slot))
            {
                informations.Add(new ProductInformationCreateDto
                {
                    ProductId = productId,
                    Type = InformationTypeEnum.Types,
                    Label = "Type",
                    Value = type.Type.Name,
                    Order = order++
                });
            }

            foreach (var ability in pokemon.Abilities.OrderBy(ability => ability.Slot))
            {
                informations.Add(new ProductInformationCreateDto
                {
                    ProductId = productId,
                    Type = InformationTypeEnum.Abilities,
                    Label = "Ability",
                    Value = ability.Ability.Name,
                    Order = order++
                });
            }

            foreach (var stat in pokemon.Stats)
            {
                informations.Add(new ProductInformationCreateDto
                {
                    ProductId = productId,
                    Type = InformationTypeEnum.Stats,
                    Label = ToTitleCase(stat.Stat.Name),
                    Value = stat.BaseStat.ToString(),
                    Order = order++
                });
            }

            foreach (var information in informations)
                await _productInformationService.AddAsync(information);
        }

        private async Task AddProductPhotosAsync(Guid productId, PokemonApiResponse pokemon)
        {
            var imageUrl = pokemon.Sprites.Other?.OfficialArtwork?.FrontDefault
                ?? pokemon.Sprites.FrontDefault;

            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            await _productPhotoService.AddAsync(new ProductPhotoCreateDto
            {
                ProductId = productId,
                FileId = $"pokeapi-{pokemon.Id}",
                Url = imageUrl,
                Description = $"{ToTitleCase(pokemon.Name)} official artwork.",
                Order = 1,
                Type = ProductPhotoEnum.Principal
            });
        }

        private async Task AddProductPriceAsync(Guid productId, PokemonApiResponse pokemon)
        {
            var basePrice = Math.Max(pokemon.BaseExperience, pokemon.Id) / 10m;

            await _productPriceService.AddAsync(new ProductPriceCreateDto
            {
                ProductId = productId,
                OriginalPrice = Math.Round(basePrice * 1.2m, 2),
                SalePrice = Math.Round(basePrice, 2),
                Currency = "BRL",
                ValidFrom = DateTime.UtcNow,
                ValidTo = null
            });
        }

        private async Task AddProductStockAsync(Guid productId, PokemonApiResponse pokemon)
        {
            await _productStockService.AddAsync(new ProductStockCreateDto
            {
                ProductId = productId,
                AvailableStock = pokemon.Id * 10L,
                ReservedStock = pokemon.Id % 3,
                SoldStock = pokemon.BaseExperience % 25
            });
        }

        private async Task<IReadOnlyList<PokemonTypeCategory>> GetOrCreateTypeCategoriesAsync(PokemonApiResponse pokemon)
        {
            var categories = new List<PokemonTypeCategory>();

            foreach (var pokemonType in pokemon.Types.OrderBy(type => type.Slot))
            {
                var category = await GetOrCreateCategoryAsync(pokemonType.Type.Name);
                categories.Add(new PokemonTypeCategory(category, pokemonType.Slot));
            }

            return categories;
        }

        private async Task<CategoryDto> GetOrCreateCategoryAsync(string typeName)
        {
            var slug = typeName.ToLowerInvariant();
            var category = await _categoryService.GetBySlugAsync(slug);

            if (category is not null)
                return category;

            return await _categoryService.AddAsync(new CategoryCreateDto
            {
                Name = ToTitleCase(typeName),
                Slug = slug,
                Description = $"Pokemon type: {ToTitleCase(typeName)}."
            });
        }

        private async Task<T> GetFromPokeApiAsync<T>(string url, CancellationToken cancellationToken)
        {
            var data = await _httpClient.GetFromJsonAsync<T>(url, cancellationToken);

            if (data is null)
                throw new InvalidOperationException($"PokeAPI returned no data for {url}.");

            return data;
        }

        private static string BuildSummary(PokemonApiResponse pokemon, PokemonSpeciesApiResponse species)
        {
            var genus = species.Genera.FirstOrDefault(g => g.Language.Name == "en")?.Genus;
            var types = string.Join(", ", pokemon.Types
                .OrderBy(type => type.Slot)
                .Select(type => type.Type.Name));
            var flavorText = GetEnglishFlavorText(species);

            return string.Join(" ", new[]
            {
                $"{ToTitleCase(pokemon.Name)} is a {genus ?? "Pokemon"} with {types} type.",
                flavorText
            }.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static string BuildDescription(PokemonApiResponse pokemon, PokemonSpeciesApiResponse species)
        {
            var abilities = string.Join(", ", pokemon.Abilities
                .OrderBy(ability => ability.Slot)
                .Select(ability => ability.Ability.Name));
            var stats = string.Join(", ", pokemon.Stats
                .Select(stat => $"{stat.Stat.Name}: {stat.BaseStat}"));
            var habitat = species.Habitat?.Name ?? "unknown";
            var color = species.Color?.Name ?? "unknown";
            var shape = species.Shape?.Name ?? "unknown";

            return string.Join(Environment.NewLine, new[]
            {
                GetEnglishFlavorText(species),
                $"Height: {pokemon.Height}. Weight: {pokemon.Weight}. Base experience: {pokemon.BaseExperience}.",
                $"Abilities: {abilities}.",
                $"Stats: {stats}.",
                $"Species details: habitat {habitat}, color {color}, shape {shape}."
            }.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static string GetEnglishFlavorText(PokemonSpeciesApiResponse species)
        {
            var text = species.FlavorTextEntries
                .FirstOrDefault(entry => entry.Language.Name == "en")
                ?.FlavorText;

            return string.IsNullOrWhiteSpace(text)
                ? string.Empty
                : text.Replace('\n', ' ').Replace('\f', ' ').Replace("  ", " ").Trim();
        }

        private static string ToTitleCase(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            return char.ToUpperInvariant(value[0]) + value[1..].Replace('-', ' ');
        }
    }

    public class PokemonSeederOptions
    {
        public const string SectionName = "PokemonSeeder";

        public bool Enabled { get; set; }
        public int StartId { get; set; } = 1;
        public int EndId { get; set; } = 300;
        public Guid? StoreId { get; set; }
        public string StoreName { get; set; } = "PokeAPI Store";
        public Guid UserCreatedId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
    }

    public record PokemonApiResponse(
        int Id,
        string Name,
        int Height,
        int Weight,
        [property: JsonPropertyName("base_experience")] int BaseExperience,
        IReadOnlyList<PokemonTypeSlot> Types,
        IReadOnlyList<PokemonAbilitySlot> Abilities,
        IReadOnlyList<PokemonStatSlot> Stats,
        PokemonSprites Sprites);

    public record PokemonTypeSlot(int Slot, NamedPokeApiResource Type);

    public record PokemonAbilitySlot(int Slot, NamedPokeApiResource Ability);

    public record PokemonStatSlot(
        [property: JsonPropertyName("base_stat")] int BaseStat,
        NamedPokeApiResource Stat);

    public record PokemonSpeciesApiResponse(
        [property: JsonPropertyName("flavor_text_entries")] IReadOnlyList<PokemonFlavorTextEntry> FlavorTextEntries,
        IReadOnlyList<PokemonGenusEntry> Genera,
        NamedPokeApiResource? Habitat,
        NamedPokeApiResource? Color,
        NamedPokeApiResource? Shape);

    public record PokemonFlavorTextEntry(
        [property: JsonPropertyName("flavor_text")] string FlavorText,
        NamedPokeApiResource Language);

    public record PokemonGenusEntry(
        string Genus,
        NamedPokeApiResource Language);

    public record NamedPokeApiResource(string Name);

    public record PokemonTypeCategory(CategoryDto Category, int Order);

    public record PokemonSprites(
        [property: JsonPropertyName("front_default")] string? FrontDefault,
        PokemonSpritesOther? Other);

    public record PokemonSpritesOther(
        [property: JsonPropertyName("official-artwork")] PokemonOfficialArtwork? OfficialArtwork);

    public record PokemonOfficialArtwork(
        [property: JsonPropertyName("front_default")] string? FrontDefault);
}
