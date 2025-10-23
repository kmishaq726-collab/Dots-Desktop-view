using System.Collections.Generic;

namespace MyApp.Models
{
    public class ApiResponse
    {
        public ApiData? Data { get; set; }
    }

    public class ApiData
    {
        public List<SaleSessionRecord>? Records { get; set; }
    }
}
