using Market.Infraestruture.DTOs;
using Microsoft.AspNetCore.Components.Authorization;

namespace Market.Infraestruture.Services
{
    public class ProductService
    {
        private readonly HttpClient _http;
        private readonly AuthenticationStateProvider _authStateProvider;

        public ProductService(IConfiguration configuration, AuthenticationStateProvider authStateProvider)
        {
            _http = new HttpClient { BaseAddress = new Uri(configuration["MarketApiUrl"]) };
            _authStateProvider = authStateProvider;
        }

        public Task<List<ProductDto>> GetFeaturedAsync()
        {
            return Task.FromResult(GetFakeProducts().Take(4).ToList());
        }

        public Task<List<ProductDto>> GetPagedAsync(int pageIndex, int pageSize)
        {
            var products = GetFakeProducts()
                .Skip(pageIndex * pageSize)
                .Take(pageSize)
                .ToList();

            return Task.FromResult(products);
        }

        public Task<ProductDto?> GetByIdAsync(Guid id)
        {
            var product = GetFakeProducts().FirstOrDefault(product => product.Id == id);
            return Task.FromResult(product);
        }

        private static List<ProductDto> GetFakeProducts()
        {
            var products = new List<ProductDto>
            {
                new ProductDto
                {
                    Id = Guid.Parse("d05d66c0-26b7-4a22-93c7-93e3f6928f12"),
                    Name = "Wireless Headphones",
                    Description = "Noise cancelling audio with long battery life.",
                    Price = 89.90m,
                    Images = new List<ImageDto>
                    {
                        new ImageDto
                        {
                            Id = Guid.Parse("ef290f2b-9f46-4f2d-9c2d-8ec02df2658c"),
                            Url = "https://placehold.co/640x460/e8f1ff/2454a6?text=Headphones",
                            IsMain = true
                        }
                    }
                },
                new ProductDto
                {
                    Id = Guid.Parse("2529f83b-50f2-4dc6-bf30-17e86d984364"),
                    Name = "Smart Watch",
                    Description = "Activity tracking, notifications, and everyday health metrics.",
                    Price = 149.50m,
                    Images = new List<ImageDto>
                    {
                        new ImageDto
                        {
                            Id = Guid.Parse("b12bfafc-b917-4716-a42b-cbf67b9304f8"),
                            Url = "https://placehold.co/640x460/eaf7ef/24734a?text=Smart+Watch",
                            IsMain = true
                        }
                    }
                },
                new ProductDto
                {
                    Id = Guid.Parse("37dbdb69-aac8-4c9a-9e58-c1db645fdc56"),
                    Name = "Portable Charger",
                    Description = "Compact USB-C fast charger for phones and accessories.",
                    Price = 34.75m,
                    Images = new List<ImageDto>
                    {
                        new ImageDto
                        {
                            Id = Guid.Parse("2f5bf861-3ab0-4d6e-8ccd-36c45113e5ef"),
                            Url = "https://placehold.co/640x460/f3efe6/7a5a20?text=Charger",
                            IsMain = true
                        }
                    }
                },
                new ProductDto
                {
                    Id = Guid.Parse("10f6845b-eb8f-4822-8bb0-b197f110964f"),
                    Name = "Desk Lamp",
                    Description = "Adjustable LED lamp with warm and cool light modes.",
                    Price = 52.30m,
                    Images = new List<ImageDto>
                    {
                        new ImageDto
                        {
                            Id = Guid.Parse("f7f8f272-30f9-460c-95a4-331f98d69c92"),
                            Url = "https://placehold.co/640x460/f7e9f0/9f2f62?text=Desk+Lamp",
                            IsMain = true
                        }
                    }
                }
            };

            var names = new[]
            {
                "Bluetooth Speaker",
                "Mechanical Keyboard",
                "Ergonomic Mouse",
                "USB-C Hub",
                "Laptop Stand",
                "Webcam Full HD",
                "Gaming Controller",
                "Smart Plug",
                "Tablet Sleeve",
                "Monitor Light Bar",
                "Travel Backpack",
                "Fitness Band",
                "Mini Projector",
                "Wireless Charger",
                "Noise Filter Mic",
                "Action Camera"
            };

            var colors = new[]
            {
                ("e8f1ff", "2454a6"),
                ("eaf7ef", "24734a"),
                ("f3efe6", "7a5a20"),
                ("f7e9f0", "9f2f62")
            };

            for (var i = 0; i < names.Length; i++)
            {
                var color = colors[i % colors.Length];
                var productNumber = i + 5;
                products.Add(new ProductDto
                {
                    Id = Guid.Parse($"00000000-0000-0000-0000-{productNumber:000000000000}"),
                    Name = names[i],
                    Description = "A useful marketplace item with practical everyday features.",
                    Price = 24.90m + (i * 8.75m),
                    Images = new List<ImageDto>
                    {
                        new ImageDto
                        {
                            Id = Guid.Parse($"10000000-0000-0000-0000-{productNumber:000000000000}"),
                            Url = $"https://placehold.co/640x460/{color.Item1}/{color.Item2}?text={Uri.EscapeDataString(names[i])}",
                            IsMain = true
                        }
                    }
                });
            }

            return products;
        }
    }
}
