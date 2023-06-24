using BankManagementSystemUI.Data;

namespace BankManagementSystemUI.Services;

public interface ICurrentAccountHolder
{
    public CustomerInfo CustomerInfo { get; set; }
    decimal Balance { get; set; }
}