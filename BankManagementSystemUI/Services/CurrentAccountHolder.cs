using BankManagementSystemUI.Data;

namespace BankManagementSystemUI.Services;

public class CurrentAccountHolder : ICurrentAccountHolder
{
    public CustomerInfo CustomerInfo { get; set; } = new CustomerInfo();
    public decimal Balance { get; set; } = 0;
}