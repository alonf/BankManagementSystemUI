using BankManagementSystemUI.Data;

namespace BankManagementSystemUI.Services;

public interface IAccountManagerClient
{
    Task<HttpResponseMessage?> RegisterCustomerAsync(CustomerRegistrationInfo customerRegistrationInfo);
    // ReSharper disable once UnusedMember.Global
    Task<AccountIdInfo?> GetAccountIdAsync(string email);
    Task<decimal?> GetAccountBalanceAsync(string accountId);
    Task<IList<AccountTransactionResponse>?> GetAccountTransactionHistory(string accountId);
    Task DepositAsync(string accountId, decimal amount);
    Task<HttpResponseMessage?> WithdrawAsync(string accountId, decimal amount);
}