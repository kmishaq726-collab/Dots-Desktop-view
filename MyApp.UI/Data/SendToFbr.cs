using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using MyApp.Models;

public class FbrService
{
    private readonly HttpClient _httpClient = new HttpClient();

    public bool IsFbrActive { get; set; } = true;
    public SaleSession SaleSession { get; set; } = new SaleSession();
    public Dictionary<string, Product> ProductsMap { get; set; } = new();

    public async Task<FbrResponse> PostToFbrAsync(List<Product> list, decimal netInvoice, decimal totalInvoice, decimal invoiceDiscount)
    {
        if (!IsFbrActive)
            throw new InvalidOperationException("FBR integration not enabled.");

        var items = new List<FbrItem>();
        decimal quantity = 0;
        decimal totalSaleValue = 0;
        decimal totalTaxCharged = 0;

        foreach (var item in list)
        {
            if (!ProductsMap.TryGetValue(item.CostCenterProductId, out var product))
                continue;

            quantity += item.Quantity;

            decimal taxCharged = 0;
            decimal itemSaleValue = item.SalePrice * item.Quantity;

            if (product.TaxType == "Inclusive")
            {
                taxCharged = (item.SalePrice * item.Quantity) -
                             ((item.SalePrice * item.Quantity) / ((100 + (decimal)product.GstRate) / 100));
                itemSaleValue -= taxCharged;
            }

            totalSaleValue += itemSaleValue;
            totalTaxCharged += taxCharged;

            items.Add(new FbrItem
            {
                ItemCode = product.GenericCode ?? product.ProductName,
                ItemName = product.ProductName,
                Quantity = item.Quantity,
                PCTCode = 1,
                TaxRate = (decimal)product.GstRate,
                SaleValue = Math.Round(itemSaleValue, 2),
                TotalAmount = item.SalePrice,
                TaxCharged = Math.Round(taxCharged, 2),
                Discount = 0.0m,
                FurtherTax = 0.0m,
                InvoiceType = 1,
                RefUSIN = null
            });
        }

        string dateString = DateTime.Now.ToString("dd/MM/yyyy hh:mm:ss tt");

        var payload = new FbrPayload
        {
            POSID = SaleSession.FbrPosId,
            USIN = "USIN0",
            DateTime = dateString,
            TotalBillAmount = netInvoice,
            TotalQuantity = quantity,
            TotalSaleValue = Math.Round(totalSaleValue, 2),
            TotalTaxCharged = Math.Round(totalTaxCharged, 2),
            Discount = invoiceDiscount,
            FurtherTax = 0.0m,
            PaymentMode = 1,
            RefUSIN = null,
            InvoiceType = 1,
            Items = items
        };

        //var url = "http://localhost:8524/api/IMSFiscal/GetInvoiceNumberByModel";
        var url = "http://192.168.100.39:8524/api/IMSFiscal/GetInvoiceNumberByModel";
        var json = JsonSerializer.Serialize(payload);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _httpClient.PostAsync(url, content);
        var responseBody = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
            throw new Exception($"FBR API Error: {response.StatusCode} - {responseBody}");

        var fbrResponse = JsonSerializer.Deserialize<FbrApiResponse>(responseBody, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });

        if (fbrResponse?.Errors != null)
            throw new Exception($"FBR returned errors: {string.Join(", ", fbrResponse.Errors)}");

        return new FbrResponse
        {
            FbrInvoiceNumber = fbrResponse?.InvoiceNumber ?? "N/A",
            TotalTaxCharged = payload.TotalTaxCharged,
            InvoiceAmount = totalInvoice,
            InvoiceDiscount = invoiceDiscount,
            InvoiceNet = Math.Round(totalSaleValue, 2)
        };
    }
    
    
}
