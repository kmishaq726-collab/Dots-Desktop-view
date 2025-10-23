// File: Common/AppConfig.cs
using System.Reflection.Metadata;

namespace MyApp.Common
{
    public static class AppConfig
    {
        // Base API URL used everywhere
        public const string BaseApiUrl = "https://dots.optimuzai.com/api/";

        // Example: common endpoint paths
        public static class Endpoints
        {
            public const string PostSale = "pos/sales/PostSale";
            public const string GetProducts = "pos/products/GetAll";
            public const string Login = "auth/login";
        }

        
    }
}
