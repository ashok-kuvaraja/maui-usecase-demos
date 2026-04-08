namespace ProductCatalogViewerApp.Models
{
    /// <summary>
    /// Represents a product category derived from the product collection.
    /// </summary>
    public class Category
    {
        /// <summary>Unique, normalized category name.</summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>Number of products in this category.</summary>
        public int ProductCount { get; set; }
    }
}
