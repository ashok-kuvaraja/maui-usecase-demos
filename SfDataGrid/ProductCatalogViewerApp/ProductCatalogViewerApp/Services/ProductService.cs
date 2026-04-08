using System.Text.Json;
using System.Text.Json.Serialization;
using ProductCatalogViewerApp.Models;

namespace ProductCatalogViewerApp.Services
{
    /// <summary>
    /// Fetches products from FakeStoreAPI with DummyJSON fallback.
    /// Validates and normalizes products at parse time.
    /// </summary>
    public class ProductService : IProductService
    {
        private const string FakeStoreApiUrl = "https://fakestoreapi.com/products";
        private const string DummyJsonUrl = "https://dummyjson.com/products?limit=100";
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(5);

        private readonly HttpClient _httpClient;
        private readonly JsonSerializerOptions _jsonOptions;

        public ProductService()
        {
            _httpClient = new HttpClient { Timeout = Timeout };
            _jsonOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                NumberHandling = JsonNumberHandling.AllowReadingFromString
            };
        }

        /// <inheritdoc />
        public async Task<List<Product>> FetchProductsAsync()
        {
            try
            {
                return await FetchFromDummyJsonAsync();
            }
            catch (Exception)
            {
                // Fallback to Fake Store
                return await FetchFromFakeStoreApiAsync();
            }
        }

        private async Task<List<Product>> FetchFromFakeStoreApiAsync()
        {
            var response = await _httpClient.GetAsync(FakeStoreApiUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var fakeStoreProducts = JsonSerializer.Deserialize<List<FakeStoreProduct>>(json, _jsonOptions)
                ?? throw new JsonException("Deserialized null from FakeStoreAPI.");

            return fakeStoreProducts
                .Select(MapFakeStoreProduct)
                .Where(p => p is not null)
                .Cast<Product>()
                .ToList();
        }

        private async Task<List<Product>> FetchFromDummyJsonAsync()
        {
            var response = await _httpClient.GetAsync(DummyJsonUrl);
            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var wrapper = JsonSerializer.Deserialize<DummyJsonWrapper>(json, _jsonOptions)
                ?? throw new JsonException("Deserialized null from DummyJSON.");

            return (wrapper.Products ?? [])
                .Select(MapDummyProduct)
                .Where(p => p is not null)
                .Cast<Product>()
                .ToList();
        }

        /// <summary>
        /// Maps and validates a FakeStoreProduct DTO to the domain Product model.
        /// Returns null if the product is invalid (missing required fields).
        /// </summary>
        private static Product? MapFakeStoreProduct(FakeStoreProduct dto)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(dto.Title)) return null;
            if (string.IsNullOrWhiteSpace(dto.Category)) return null;
            if (dto.Price < 0 || dto.Price > 999999.99m) return null;

            // Validate optional image URL
            string? image = null;
            if (!string.IsNullOrWhiteSpace(dto.Image) &&
                Uri.TryCreate(dto.Image, UriKind.Absolute, out var uri) &&
                (uri.Scheme == "http" || uri.Scheme == "https"))
            {
                image = dto.Image;
            }

            // Validate optional rating
            decimal? rating = null;
            int? ratingCount = null;
            if (dto.Rating is not null)
            {
                if (dto.Rating.Rate >= 0 && dto.Rating.Rate <= 5)
                    rating = dto.Rating.Rate;
                if (dto.Rating.Count >= 0)
                    ratingCount = dto.Rating.Count;
            }

            return new Product
            {
                Id = dto.Id,
                Title = dto.Title.Trim(),
                Category = dto.Category.Trim().ToLowerInvariant(),
                Price = dto.Price,
                Description = dto.Description,
                Image = image,
                Rating = rating,
                RatingCount = ratingCount
            };
        }

        /// <summary>
        /// Maps and validates a DummyJsonProduct DTO to the domain Product model.
        /// </summary>
        private static Product? MapDummyProduct(DummyJsonProduct dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Title)) return null;
            if (string.IsNullOrWhiteSpace(dto.Category)) return null;
            if (dto.Price < 0) return null;

            string? image = null;
            if (!string.IsNullOrWhiteSpace(dto.Thumbnail) &&
                Uri.TryCreate(dto.Thumbnail, UriKind.Absolute, out var uri) &&
                (uri.Scheme == "http" || uri.Scheme == "https"))
            {
                image = dto.Thumbnail;
            }

            decimal? rating = null;
            if (dto.Rating is >= 0 and <= 5)
                rating = dto.Rating;

            return new Product
            {
                Id = dto.Id,
                Title = dto.Title.Trim(),
                Category = dto.Category.Trim().ToLowerInvariant(),
                Price = dto.Price,
                Description = dto.Description,
                Image = image,
                Rating = rating,
                Stock = dto.Stock
            };
        }

        // ─── DummyJSON DTOs ──────────────────────────────────────────────────────

        private class DummyJsonWrapper
        {
            [JsonPropertyName("products")]
            public List<DummyJsonProduct>? Products { get; set; }
        }

        private class DummyJsonProduct
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }

            [JsonPropertyName("title")]
            public string? Title { get; set; }

            [JsonPropertyName("price")]
            public decimal Price { get; set; }

            [JsonPropertyName("description")]
            public string? Description { get; set; }

            [JsonPropertyName("category")]
            public string? Category { get; set; }

            [JsonPropertyName("thumbnail")]
            public string? Thumbnail { get; set; }

            [JsonPropertyName("rating")]
            public decimal? Rating { get; set; }

            [JsonPropertyName("stock")]
            public int? Stock { get; set; }
        }
    }
}
