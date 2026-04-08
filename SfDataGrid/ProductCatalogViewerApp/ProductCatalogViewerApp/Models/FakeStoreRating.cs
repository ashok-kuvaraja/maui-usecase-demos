using System.Text.Json.Serialization;

namespace ProductCatalogViewerApp.Models
{
    /// <summary>
    /// DTO for the nested rating object from FakeStoreAPI.
    /// </summary>
    public class FakeStoreRating
    {
        [JsonPropertyName("rate")]
        public decimal Rate { get; set; }

        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
