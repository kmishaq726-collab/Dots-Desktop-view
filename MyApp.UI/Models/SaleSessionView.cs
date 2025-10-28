namespace MyApp.Models
{
    public sealed class SaleSessionView
    {
        public string? SaleSessionId { get; set; } = string.Empty;
        public string? Channel { get; set; } = string.Empty;
        public string? StartTime { get; set; } = string.Empty;
        public string? EndTime { get; set; } = string.Empty;
        public string? SessionState { get; set; } = string.Empty;
    }
}
