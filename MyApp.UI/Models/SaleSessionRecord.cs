using MyApp.Models;

namespace MyApp.Models
{
    public class SaleSessionRecord
    {
        public string SaleSessionId { get; set; } = string.Empty;
        public long StartTime { get; set; }
        public long? EndTime { get; set; }
        public string SessionState { get; set; } = string.Empty;
        public SaleChannel? SaleChannel { get; set; }
    }
}
