namespace BankManagementSystemUI.Data;

public class AccountTransactionResponse
{
    public string? AccountId { get; set; }

    public decimal TransactionAmount { get; set; }

    public DateTimeOffset TransactionTime { get; set; }
}