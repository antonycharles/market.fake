using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using Market.Application.DTOs;
using Market.Application.Interfaces;
using Market.Domain.Enums;
using Market.Domain.Exceptions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace Market.Api.Seeders
{
    public class DigimonProductSeeder
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
        private readonly ILogger<DigimonProductSeeder> _logger;

        public DigimonProductSeeder(
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
            ILogger<DigimonProductSeeder> logger)
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
                .GetSection(DigimonSeederOptions.SectionName)
                .Get<DigimonSeederOptions>() ?? new DigimonSeederOptions();

            if (!options.Enabled)
                return;

            var storeId = await GetStoreIdAsync(options);

            for (var id = options.StartId; id <= options.EndId; id++)
            {
                try
                {
                    var digimon = await GetFromDigiApiAsync<DigimonApiResponse>(
                        $"{options.BaseUrl.TrimEnd('/')}/{id}",
                        cancellationToken);

                    var categories = await GetOrCreateTypeCategoriesAsync(digimon);
                    var product = await _productService.AddAsync(new ProductCreateDto
                    {
                        StoreId = storeId,
                        Name = digimon.Name,
                        Code = checked(options.CodeOffset + digimon.Id),
                        Slug = ToSlug(digimon.Name),
                        Summary = BuildSummary(digimon),
                        Description = BuildDescription(digimon)
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

                    await AddProductInformationsAsync(product.Id, digimon);
                    await AddProductPhotoAsync(product.Id, digimon);
                    await AddProductPriceAsync(product.Id, digimon);
                    await AddProductStockAsync(product.Id, digimon);
                }
                catch (BusinessException ex) when (ex.Message.Contains("code already exists", StringComparison.OrdinalIgnoreCase))
                {
                    _logger.LogInformation("Digimon product {DigimonId} already exists. Skipping.", id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Could not seed digimon product {DigimonId}.", id);
                }
            }
        }

        private async Task<Guid> GetStoreIdAsync(DigimonSeederOptions options)
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
                Description = "Store created by the Digimon product seeder.",
                UserCreatedId = options.UserCreatedId
            });

            return store.Id;
        }

        private async Task AddProductInformationsAsync(Guid productId, DigimonApiResponse digimon)
        {
            var order = 1;
            var informations = new List<ProductInformationCreateDto>
            {
                NewInformation(productId, InformationTypeEnum.Id, "Digi-API Id", digimon.Id.ToString(), order++),
                NewInformation(productId, InformationTypeEnum.XAntibody, "X-Antibody", digimon.XAntibody ? "Yes" : "No", order++)
            };

            if (!string.IsNullOrWhiteSpace(digimon.ReleaseDate))
            {
                informations.Add(NewInformation(
                    productId,
                    InformationTypeEnum.ReleaseDate,
                    "Release Date",
                    digimon.ReleaseDate,
                    order++));
            }

            foreach (var level in digimon.Levels)
                informations.Add(NewInformation(productId, InformationTypeEnum.Levels, "Level", level.Level, order++));

            foreach (var type in digimon.Types)
                informations.Add(NewInformation(productId, InformationTypeEnum.Types, "Type", type.Type, order++));

            foreach (var attribute in digimon.Attributes)
                informations.Add(NewInformation(productId, InformationTypeEnum.Attributes, "Attribute", attribute.Attribute, order++));

            foreach (var field in digimon.Fields)
                informations.Add(NewInformation(productId, InformationTypeEnum.Fields, "Field", field.Field, order++));

            foreach (var skill in digimon.Skills)
            {
                var value = string.IsNullOrWhiteSpace(skill.Translation)
                    ? skill.Skill
                    : $"{skill.Skill} ({skill.Translation})";

                informations.Add(NewInformation(productId, InformationTypeEnum.Abilities, "Skill", value, order++));
            }

            foreach (var information in informations)
                await _productInformationService.AddAsync(information);
        }

        private async Task AddProductPhotoAsync(Guid productId, DigimonApiResponse digimon)
        {
            var imageUrl = digimon.Images.FirstOrDefault()?.Href;

            if (string.IsNullOrWhiteSpace(imageUrl))
                return;

            await _productPhotoService.AddAsync(new ProductPhotoCreateDto
            {
                ProductId = productId,
                FileId = $"digi-api-{digimon.Id}",
                Url = imageUrl,
                Description = $"{digimon.Name} official artwork.",
                Order = 1,
                Type = ProductPhotoEnum.Principal
            });
        }

        private async Task AddProductPriceAsync(Guid productId, DigimonApiResponse digimon)
        {
            var basePrice = Math.Max(
                digimon.Id,
                (digimon.Skills.Count * 10) + (digimon.Levels.Count * 25)) / 10m;

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

        private async Task AddProductStockAsync(Guid productId, DigimonApiResponse digimon)
        {
            await _productStockService.AddAsync(new ProductStockCreateDto
            {
                ProductId = productId,
                AvailableStock = digimon.Id * 10L,
                ReservedStock = digimon.Id % 3,
                SoldStock = digimon.Skills.Count % 25
            });
        }

        private async Task<IReadOnlyList<DigimonTypeCategory>> GetOrCreateTypeCategoriesAsync(DigimonApiResponse digimon)
        {
            var categories = new List<DigimonTypeCategory>();

            for (var index = 0; index < digimon.Types.Count; index++)
            {
                var type = digimon.Types[index];
                var category = await GetOrCreateCategoryAsync(type.Type);
                categories.Add(new DigimonTypeCategory(category, index + 1));
            }

            return categories;
        }

        private async Task<CategoryDto> GetOrCreateCategoryAsync(string typeName)
        {
            var slug = $"digimon-{ToSlug(typeName)}";
            var existingCategory = await FindCategoryBySlugAsync(slug);

            if (existingCategory is not null)
                return existingCategory;

            try
            {
                return await _categoryService.AddAsync(new CategoryCreateDto
                {
                    Name = typeName,
                    Slug = slug,
                    Description = $"Digimon type: {typeName}."
                });
            }
            catch (BusinessException ex) when (ex.Message.Contains("slug already exists", StringComparison.OrdinalIgnoreCase))
            {
                var category = await FindCategoryBySlugAsync(slug);

                if (category is not null)
                    return category;

                throw;
            }
        }

        private async Task<CategoryDto?> FindCategoryBySlugAsync(string slug)
        {
            var categories = await _categoryService.GetPagedAsync(
                new PaginationRequestDto { PageIndex = 1, PageSize = 1000 });

            return categories.Items.FirstOrDefault(category =>
                string.Equals(category.Slug, slug, StringComparison.OrdinalIgnoreCase));
        }

        private async Task<T> GetFromDigiApiAsync<T>(string url, CancellationToken cancellationToken)
        {
            var data = await _httpClient.GetFromJsonAsync<T>(url, cancellationToken);

            if (data is null)
                throw new InvalidOperationException($"Digi-API returned no data for {url}.");

            return data;
        }

        private static ProductInformationCreateDto NewInformation(
            Guid productId,
            InformationTypeEnum type,
            string label,
            string value,
            int order) => new()
            {
                ProductId = productId,
                Type = type,
                Label = label,
                Value = value,
                Order = order
            };

        private static string BuildSummary(DigimonApiResponse digimon)
        {
            var level = string.Join(", ", digimon.Levels.Select(item => item.Level));
            var type = string.Join(", ", digimon.Types.Select(item => item.Type));
            var attribute = string.Join(", ", digimon.Attributes.Select(item => item.Attribute));

            return string.Join(" ", new[]
            {
                $"{digimon.Name} is a {(string.IsNullOrWhiteSpace(type) ? "Digimon" : type)} Digimon.",
                string.IsNullOrWhiteSpace(level) ? null : $"Level: {level}.",
                string.IsNullOrWhiteSpace(attribute) ? null : $"Attribute: {attribute}.",
                GetEnglishDescription(digimon)
            }.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static string BuildDescription(DigimonApiResponse digimon)
        {
            var skills = string.Join(", ", digimon.Skills.Select(skill => skill.Skill));
            var fields = string.Join(", ", digimon.Fields.Select(field => field.Field));

            return string.Join(Environment.NewLine, new[]
            {
                GetEnglishDescription(digimon),
                string.IsNullOrWhiteSpace(digimon.ReleaseDate) ? null : $"Release date: {digimon.ReleaseDate}.",
                $"X-Antibody: {(digimon.XAntibody ? "yes" : "no")}.",
                string.IsNullOrWhiteSpace(fields) ? null : $"Fields: {fields}.",
                string.IsNullOrWhiteSpace(skills) ? null : $"Skills: {skills}."
            }.Where(value => !string.IsNullOrWhiteSpace(value)));
        }

        private static string GetEnglishDescription(DigimonApiResponse digimon) =>
            digimon.Descriptions.FirstOrDefault(description =>
                description.Language.StartsWith("en", StringComparison.OrdinalIgnoreCase))?.Description ?? string.Empty;

        private static string ToSlug(string value)
        {
            var builder = new StringBuilder();
            var addSeparator = false;

            foreach (var character in value.ToLowerInvariant())
            {
                if (char.IsLetterOrDigit(character))
                {
                    if (addSeparator && builder.Length > 0)
                        builder.Append('-');

                    builder.Append(character);
                    addSeparator = false;
                }
                else
                {
                    addSeparator = true;
                }
            }

            return builder.ToString();
        }
    }

    public class DigimonSeederOptions
    {
        public const string SectionName = "DigimonSeeder";

        public bool Enabled { get; set; }
        public int StartId { get; set; } = 1;
        public int EndId { get; set; } = 300;
        public int CodeOffset { get; set; } = 100_000;
        public string BaseUrl { get; set; } = "https://digi-api.com/api/v1/digimon";
        public Guid? StoreId { get; set; }
        public string StoreName { get; set; } = "Digi-API Store";
        public Guid UserCreatedId { get; set; } = Guid.Parse("00000000-0000-0000-0000-000000000001");
    }

    public record DigimonApiResponse(
        int Id,
        string Name,
        [property: JsonPropertyName("xAntibody")] bool XAntibody,
        IReadOnlyList<DigimonImage> Images,
        IReadOnlyList<DigimonLevel> Levels,
        IReadOnlyList<DigimonType> Types,
        IReadOnlyList<DigimonAttribute> Attributes,
        IReadOnlyList<DigimonField> Fields,
        string? ReleaseDate,
        IReadOnlyList<DigimonDescription> Descriptions,
        IReadOnlyList<DigimonSkill> Skills);

    public record DigimonImage(string Href, bool Transparent);

    public record DigimonLevel(int Id, string Level);

    public record DigimonType(int Id, string Type);

    public record DigimonAttribute(int Id, string Attribute);

    public record DigimonField(int Id, string Field, string? Image);

    public record DigimonDescription(string Origin, string Language, string Description);

    public record DigimonSkill(int Id, string Skill, string? Translation, string? Description);

    public record DigimonTypeCategory(CategoryDto Category, int Order);
}
