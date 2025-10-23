namespace MyApp.UI.Data
{
    public class PostSaleRecord
    {
        public int Id { get; set; }
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public DateTime LastUpdated { get; set; }
    }
}
