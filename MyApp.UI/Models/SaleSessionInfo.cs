using System;
using System.Collections.Generic;

namespace MyApp.Models
{
    public class SaleSessionInfo
    {
        public SaleSessionInfoData? Data { get; set; }
        public string? Status { get; set; }
        public long Time { get; set; }
        public string? ProcTime { get; set; }
    }

    public class SaleSessionInfoData
    {
        public List<PaymentMethod>? PaymentMethods { get; set; }
        public List<Product>? Products { get; set; }
        public List<Customer>? Customers { get; set; }
        public PrintSettings? PrintSettings { get; set; }
        public SaleSession? SaleSession { get; set; }
        public CostCenters? CostCenters { get; set; }
    }

    // ---------------- Payment Method ----------------
    public class PaymentMethod
    {
        public string? SalePaymentMethodId { get; set; }
        public string? SalePaymentMethodName { get; set; }
        public string? Type { get; set; }
    }

    // ---------------- Product ----------------
    public class Product
    {
        public string? CostCenterProductId { get; set; }
        public string? ProductId { get; set; }
        public string? ProductGroupId { get; set; }
        public string? ProductTypeId { get; set; }
        public string? ProductBrandId { get; set; }
        public string? GenericCode { get; set; }
        public string? ProductName { get; set; }
        public string? ProductBarCode { get; set; }
        public string? ProductBarCodeManual { get; set; }
        public decimal SalePrice { get; set; }
        public string? TaxType { get; set; }
        public double GstRate { get; set; }
        public bool AddOrderNumber { get; set; }
        public int Quantity { get; set; } = 0;

        public double LineDiscount { get; set; } = 0.00;
    }

    // ---------------- Customer ----------------
    public class Customer
    {
        public string? CustomerId { get; set; }
        public string? FullName { get; set; }
        public string? Phone1 { get; set; }
        public string? Cnic { get; set; }
    }

    // ---------------- Print Settings ----------------
    public class PrintSettings
    {
        public string? CostCenterId { get; set; }
        public string? Name { get; set; }
        public string? Type { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? Remarks { get; set; }
        public string? Website { get; set; }
        public bool KitchenReceipt { get; set; }
        public bool CreatePO { get; set; }
        public bool CustomerRequired { get; set; }
    }

    // ---------------- Sale Session ----------------
    public class SaleSession
    {
        public string? SaleSessionId { get; set; }
        public string? SaleChannelId { get; set; }
        public string? CostCenterId { get; set; }
        public string? CreatedById { get; set; }
        public long StartTime { get; set; }
        public long? EndTime { get; set; }
        public string? SessionState { get; set; }
        public double SessionOpeningCash { get; set; }
        public double SessionClosingCash { get; set; }
        public double InvoicesNetTotal { get; set; }
        public double Difference { get; set; }
        public string? Notes { get; set; }
        public string? FbrPosId { get; set; }
        public bool IsActive { get; set; }
        public long DateCreated { get; set; }
        public long DateUpdated { get; set; }
    }

    // ---------------- Cost Center ----------------
    public class CostCenters
    {
        public string? CostCenterId { get; set; }
        public string? BusinessUnitId { get; set; }
        public string? CashAccountId { get; set; }
        public string? CostCenterName { get; set; }
        public string? CostCenterCode { get; set; }
        public int StartTime { get; set; }
        public int EndTime { get; set; }
        public int TimezoneOffset { get; set; }
        public string? Description { get; set; }
        public bool ProductionFacility { get; set; }
        public bool EnableFbrIntegration { get; set; }
        public string? Strn { get; set; }
        public string? Ntn { get; set; }
        public bool IsActive { get; set; }
        public long DateCreated { get; set; }
        public long DateUpdated { get; set; }
    }
}
