using System;
using System.Collections.Generic;

namespace MyApp.Models
{
    public class Profile
    {
        public string? EmployeeId { get; set; }
        public string? AccountId { get; set; }
        public string? FullName { get; set; }
        public string? Image { get; set; }
        public string? Email { get; set; }
        public string? Username { get; set; }
        public string? Group { get; set; }
    }

    public class CostCenterActive
    {
        public string? CostCenterId { get; set; }
        public string? BusinessUnitId { get; set; }
        public string? CashAccountId { get; set; }
        public string? CostCenterName { get; set; }
        public string? CostCenterCode { get; set; }
        public int? StartTime { get; set; }
        public int? EndTime { get; set; }
        public int? TimezoneOffset { get; set; }
        public string? Description { get; set; }
        public bool? ProductionFacility { get; set; }
        public bool EnableFbrIntegration { get; set; }
        public string? Strn { get; set; }
        public string? Ntn { get; set; }
        public bool? IsActive { get; set; }
        public long? DateCreated { get; set; }
        public long? DateUpdated { get; set; }
    }

    public class CostCenter
    {
        public CostCenterActive? Active { get; set; }
        public List<object>? Others { get; set; }
    }

    public class BaseCurrency
    {
        public string? CurrencyId { get; set; }
        public string? CurrencyCode { get; set; }
        public string? CurrencySymbol { get; set; }
        public string? CurrencyName { get; set; }
        public bool? IsActive { get; set; }
        public long? DateCreated { get; set; }
        public long? DateUpdated { get; set; }
    }

    public class SystemConfigData
    {
        public Profile? Profile { get; set; }
        public CostCenter? CostCenter { get; set; }
        public bool? WizardCompleted { get; set; }
        public object? Permissions { get; set; }
        public BaseCurrency? BaseCurrency { get; set; }
    }

    public class SystemConfigResponse
    {
        public SystemConfigData? Data { get; set; }
        public string? Status { get; set; }
        public long? Time { get; set; }
        public string? ProcTime { get; set; }
    }
}
