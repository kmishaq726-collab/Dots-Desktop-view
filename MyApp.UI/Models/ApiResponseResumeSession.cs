namespace MyApp.Models
{
    public class ApiResponse_ResumeSession
    {
        public ResumeSessionData? Data { get; set; }
        public string? Status { get; set; }
        public long Time { get; set; }
        public string? ProcTime { get; set; }
    }

    public class ResumeSessionData
    {
        public string? SaleSessionId { get; set; } = string.Empty;
        public string? Pk { get; set; }
        public string? PosToken { get; set; }
    }
}
