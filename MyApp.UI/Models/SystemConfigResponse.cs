using System;
using System.Collections.Generic;

namespace MyApp.UI.Models
{
    public class Profile
    {
        public string EmployeeId { get; set; } = string.Empty;
        public string AccountId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string? Image { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Username { get; set; } = string.Empty;
        public string Group { get; set; } = string.Empty;
    }

    public class CostCenterActive
    {
        public string CostCenterId { get; set; } = string.Empty;
        public string BusinessUnitId { get; set; } = string.Empty;
        public string CashAccountId { get; set; } = string.Empty;
        public string CostCenterName { get; set; } = string.Empty;
        public string CostCenterCode { get; set; } = string.Empty;
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

    public class CostCenter
    {
        public CostCenterActive Active { get; set; } = new();
        public List<object> Others { get; set; } = new();
    }

    public class BaseCurrency
    {
        public string CurrencyId { get; set; } = string.Empty;
        public string CurrencyCode { get; set; } = string.Empty;
        public string CurrencySymbol { get; set; } = string.Empty;
        public string CurrencyName { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public long DateCreated { get; set; }
        public long DateUpdated { get; set; }
    }

    public class SystemConfigData
    {
        public Profile Profile { get; set; } = new();
        public CostCenter CostCenter { get; set; } = new();
        public bool WizardCompleted { get; set; }
        public object Permissions { get; set; } = new();
        public BaseCurrency BaseCurrency { get; set; } = new();
    }

    public class SystemConfigResponse
    {
        public SystemConfigData Data { get; set; } = new();
        public string Status { get; set; } = string.Empty;
        public long Time { get; set; }
        public string ProcTime { get; set; } = string.Empty;
    }
}

