using BankManagementSystemUI.Data;

namespace BankManagementSystemUI.Services
{
    public interface IAccountManagerClient
    {
        Task<HttpResponseMessage?> RegisterCustomerAsync(CustomerRegistrationInfo customerRegistrationInfo);
        Task<AccountIdInfo?> GetAccountIdAsync(string email);
        Task<decimal?> GetAccountBalanceAsync(string accountId);
        Task<IList<AccountTransactionResponse>?> GetAccountTransactionHistory(string accountId);
        Task DepositAsync(string accountId, decimal amount);
        Task WithdrawAsync(string accountId, decimal amount);
    }
}
