using System;
using System.Collections.Generic;
using System.Linq;
using MyApp.Models;
using MyApp.UI.Data;

namespace MyApp.UI.Services
{
    public static class PinnedProductsService
    {
        private static List<PinnedProduct> _pinnedProducts = new List<PinnedProduct>();

        public static event Action<List<PinnedProduct>> PinnedProductsChanged;

        public class PinnedProduct
        {
            public string ProductName { get; set; }
            public decimal Price { get; set; }
        }

        public static void PinSavedProducts(List<PinnedProduct> products)
        {
            _pinnedProducts = products ?? new List<PinnedProduct>();
            PinnedProductsChanged?.Invoke(new List<PinnedProduct>(_pinnedProducts));
        }

        public static void PinProduct(string productName, decimal price)
        {
            // Remove if already exists (to update price if needed)
            _pinnedProducts.RemoveAll(p => p.ProductName == productName);

            // Add new product
            _pinnedProducts.Add(new PinnedProduct { ProductName = productName, Price = price });

            // Notify all subscribers
            PinnedProductsChanged?.Invoke(new List<PinnedProduct>(_pinnedProducts));

            SystemConfigRepository.SaveConfig("_pinnedProducts", _pinnedProducts);
        }

        public static void UnpinProduct(string productName)
        {
            _pinnedProducts.RemoveAll(p => p.ProductName == productName);
            PinnedProductsChanged?.Invoke(new List<PinnedProduct>(_pinnedProducts));
             SystemConfigRepository.SaveConfig("_pinnedProducts", _pinnedProducts);
        }

        public static List<PinnedProduct> GetPinnedProducts()
        {
            return new List<PinnedProduct>(_pinnedProducts);
        }

        public static void ClearAll()
        {
            _pinnedProducts.Clear();
            PinnedProductsChanged?.Invoke(new List<PinnedProduct>());
        }
    }
}