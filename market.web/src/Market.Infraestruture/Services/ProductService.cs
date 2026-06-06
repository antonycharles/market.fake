using Market.Infraestruture.DTOs;
using Market.Infraestruture.Enums;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace Market.Infraestruture.Services
{
    public class ProductService
    {
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;

        public ProductService(IConfiguration configuration, AuthenticationStateProvider authStateProvider)
        {
            var marketApiUrl = configuration["MarketApiUrl"]
                ?? throw new InvalidOperationException("MarketApiUrl setting is required.");

            _http = new HttpClient { BaseAddress = new Uri(marketApiUrl) };
            _authStateProvider = authStateProvider;
        }

        public Task<List<ProductDto>> GetFeaturedAsync()
        {
            return GetPagedAsync(0, 8, null, null, ProductOrderEnum.BestSellingDesc);
        }

        public async Task<List<ProductDto>> GetPagedAsync(int pageIndex, int pageSize, Guid? categoryId = null, string? search = null, ProductOrderEnum? orderBy = null)
        {
            var response = await GetPagedResponseAsync(pageIndex, pageSize, categoryId, search, orderBy);
            return response.Items;
        }

        public async Task<PaginatedResponse<ProductDto>> GetPagedResponseAsync(int pageIndex, int pageSize, Guid? categoryId = null, string? search = null, ProductOrderEnum? orderBy = null)
        {
            var apiPageIndex = pageIndex + 1;
            var query = $"v1/Products?PageIndex={apiPageIndex}&PageSize={pageSize}";

            if (categoryId.HasValue)
            {
                query += $"&CategoryId={categoryId.Value}";
            }

            if (!string.IsNullOrWhiteSpace(search))
            {
                query += $"&Search={Uri.EscapeDataString(search.Trim())}";
            }

            if (orderBy.HasValue)
            {
                query += $"&Order={orderBy.Value}";
            }

            var response = await _http.GetFromJsonAsync<PaginatedResponse<ProductListItemDto>>(
                query);

            return new PaginatedResponse<ProductDto>
            {
                Items = response?.Items?.Select(MapProductListItem).ToList() ?? new List<ProductDto>(),
                TotalItems = response?.TotalItems ?? 0,
                PageIndex = response?.PageIndex ?? apiPageIndex,
                PageSize = response?.PageSize ?? pageSize,
                HasNextPage = response?.HasNextPage ?? false
            };
        }

        public async Task<ProductDto?> GetByIdAsync(Guid id)
        {
            await AddBearerTokenAsync();
            var product = await _http.GetFromJsonAsync<ProductDetailsDto>($"v1/Products/{id}");

            return product is null
                ? null
                : new ProductDto
                {
                    Id = product.Id,
                    Code = product.Code,
                    Slug = product.Slug,
                    Name = product.Name,
                    Summary = product.Summary ?? string.Empty,
                    Description = product.Description ?? product.Summary ?? string.Empty,
                    Price = 0,
                    Currency = "BRL",
                    Images = new List<ImageDto>()
                };
        }

        public async Task<ProductDetailsViewDto?> GetDetailsBySlugAsync(string slug)
        {
            if (string.IsNullOrWhiteSpace(slug))
            {
                return null;
            }

            var searchResult = await GetPagedResponseAsync(0, 20, search: slug);
            var product = searchResult.Items.FirstOrDefault(product =>
                string.Equals(product.Slug, slug, StringComparison.OrdinalIgnoreCase));

            return product is null ? null : await GetDetailsAsync(product.Id);
        }

        public async Task<ProductDetailsViewDto?> GetDetailsAsync(Guid id)
        {
            await AddBearerTokenAsync();

            var product = await _http.GetFromJsonAsync<ProductDetailsDto>($"v1/Products/{id}");

            if (product is null)
            {
                return null;
            }

            var photosTask = GetOrDefaultAsync<List<ProductPhotoDto>>($"v1/ProductPhotos/product/{id}", new List<ProductPhotoDto>());
            var priceTask = GetOrDefaultAsync<ProductPriceDto?>($"v1/ProductPrices/product/{id}/current", null);
            var informationsTask = GetOrDefaultAsync<List<ProductInformationDto>>($"v1/ProductInformations/product/{id}", new List<ProductInformationDto>());
            var stockTask = GetOrDefaultAsync<ProductStockDto?>($"v1/ProductStocks/product/{id}", null);

            await Task.WhenAll(photosTask, priceTask, informationsTask, stockTask);

            var photos = photosTask.Result.OrderBy(photo => photo.Order).ToList();
            var price = priceTask.Result;

            return new ProductDetailsViewDto
            {
                Product = new ProductDto
                {
                    Id = product.Id,
                    Code = product.Code,
                    Slug = product.Slug,
                    Name = product.Name,
                    Summary = product.Summary ?? string.Empty,
                    Description = product.Description ?? product.Summary ?? string.Empty,
                    Price = price?.SalePrice ?? price?.OriginalPrice ?? 0,
                    Currency = price?.Currency ?? "BRL",
                    Images = photos.Select(photo => new ImageDto
                    {
                        Id = photo.Id,
                        Url = photo.Url,
                        IsMain = photo.Type == 1
                    }).ToList()
                },
                Price = price,
                Photos = photos,
                Informations = informationsTask.Result.OrderBy(info => info.Order).ToList(),
                Stock = stockTask.Result
            };
        }

        private async Task<T> GetOrDefaultAsync<T>(string url, T fallback)
        {
            var response = await _http.GetAsync(url);

            if (response.StatusCode == HttpStatusCode.NotFound)
            {
                return fallback;
            }

            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<T>() ?? fallback;
        }

        private async Task AddBearerTokenAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity?.IsAuthenticated != true)
            {
                return;
            }

            var token = user.FindFirst("access_token")?.Value;
            if (!string.IsNullOrEmpty(token))
            {
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        private static ProductDto MapProductListItem(ProductListItemDto product)
        {
            return new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Code = product.Code,
                Slug = product.Slug,
                Summary = product.Summary ?? string.Empty,
                Description = product.Summary ?? string.Empty,
                Price = product.ProductPrice?.SalePrice ?? product.ProductPrice?.OriginalPrice ?? 0,
                Currency = product.ProductPrice?.Currency ?? "BRL",
                Images = product.ProductPhoto is null
                    ? new List<ImageDto>()
                    : new List<ImageDto>
                    {
                        new()
                        {
                            Id = product.ProductPhoto.Id,
                            Url = product.ProductPhoto.Url,
                            IsMain = true
                        }
                    }
            };
        }
    }
}
