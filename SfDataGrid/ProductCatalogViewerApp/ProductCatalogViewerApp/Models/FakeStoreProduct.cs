using System.Text.Json.Serialization;

namespace ProductCatalogViewerApp.Models
{
    /// <summary>
    /// DTO for deserializing a product from FakeStoreAPI JSON response.
    /// </summary>
    public class FakeStoreProduct
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

        [JsonPropertyName("image")]
        public string? Image { get; set; }

        [JsonPropertyName("rating")]
        public FakeStoreRating? Rating { get; set; }
    }
}
