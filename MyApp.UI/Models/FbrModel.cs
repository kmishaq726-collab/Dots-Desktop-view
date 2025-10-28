public class FbrPayload
{
    public string? POSID { get; set; }
    public string? USIN { get; set; }
    public string? DateTime { get; set; }
    public decimal? TotalBillAmount { get; set; }
    public decimal? TotalQuantity { get; set; }
    public decimal? TotalSaleValue { get; set; }
    public decimal? TotalTaxCharged { get; set; }
    public decimal? Discount { get; set; }
    public decimal? FurtherTax { get; set; }
    public int? PaymentMode { get; set; }
    public string? RefUSIN { get; set; }
    public int? InvoiceType { get; set; }
    public List<FbrItem>? Items { get; set; }
}

public class FbrItem
{
    public string? ItemCode { get; set; }
    public string? ItemName { get; set; }
    public decimal? Quantity { get; set; }
    public int? PCTCode { get; set; }
    public decimal? TaxRate { get; set; }
    public decimal? SaleValue { get; set; }
    public decimal? TotalAmount { get; set; }
    public decimal? TaxCharged { get; set; }
    public decimal? Discount { get; set; }
    public decimal? FurtherTax { get; set; }
    public int? InvoiceType { get; set; }
    public string? RefUSIN { get; set; }
}

public class FbrApiResponse
{
    public string? InvoiceNumber { get; set; }
    public List<string>? Errors { get; set; }
}

public class FbrResponse
{
    public string? FbrInvoiceNumber { get; set; }
    public string? QrCodeBase64 { get; set; }
    public decimal? TotalTaxCharged { get; set; }
    public decimal? InvoiceAmount { get; set; }
    public decimal? InvoiceDiscount { get; set; }
    public decimal? InvoiceNet { get; set; }
}

public class FbrPrintData
{
    public decimal? TotalTaxCharged { get; set; }
    public string? FbrInvoiceNumber { get; set; }
    public string? QrCodeBase64 { get; set; }
}
