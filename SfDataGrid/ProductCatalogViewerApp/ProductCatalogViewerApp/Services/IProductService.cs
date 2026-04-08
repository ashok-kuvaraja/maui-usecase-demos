using ProductCatalogViewerApp.Models;

namespace ProductCatalogViewerApp.Services
{
    /// <summary>
    /// Contract for fetching product data from the remote API.
    /// </summary>
    public interface IProductService
    {
        /// <summary>
        /// Fetches all products from FakeStoreAPI (with DummyJSON fallback).
        /// </summary>
        /// <returns>List of validated products, or empty list on failure.</returns>
        /// <exception cref="HttpRequestException">Thrown on network/HTTP errors.</exception>
        /// <exception cref="TaskCanceledException">Thrown on timeout.</exception>
        Task<List<Product>> FetchProductsAsync();
    }
}
