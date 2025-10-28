namespace MyApp.Models
{
    public class PosSale
    {
        public string? SaleSessionId { get; set; } = string.Empty;
        public decimal? InvoiceAmount { get; set; }
        public decimal? InvoiceDiscount { get; set; }
        public decimal? InvoiceNet { get; set; }
        public string? CustomerId { get; set; }
        public long? InvoiceDate { get; set; } // Unix timestamp (e.g., 1761008987)
        public string? FbrInvoiceNumber { get; set; }

        public List<SalePaymentMethod>? SalePaymentMethods { get; set; } = new();
        public List<Product>? Items { get; set; } = new();
    }

    public class SalePaymentMethod
    {
        public string? SalePaymentMethodId { get; set; } = string.Empty;
        public decimal? Amount { get; set; }
    }

}
