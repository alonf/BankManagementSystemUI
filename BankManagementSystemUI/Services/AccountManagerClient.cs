﻿using BankManagementSystemUI.Data;
using System.Text.Json;
using System.Text;

namespace BankManagementSystemUI.Services;

public class AccountManagerClient : IAccountManagerClient
{
    private readonly INetworkServerProvider _networkServerProvider;
    private readonly ILogger _logger;
    private readonly ISignalRWrapper _signalRWrapper;

    public AccountManagerClient(INetworkServerProvider networkServerProvider,
        ILogger<AccountManagerClient> logger, ISignalRWrapper signalRWrapper)
    {
        _networkServerProvider = networkServerProvider;
        _logger = logger;
        _signalRWrapper = signalRWrapper;
            
    }

    private JsonSerializerOptions SerializeOptions => new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
        
    public async Task<HttpResponseMessage?> RegisterCustomerAsync(CustomerRegistrationInfo customerRegistrationInfo)
    {
        await _signalRWrapper.StartSignalRAsync();
        var json = JsonSerializer.Serialize(customerRegistrationInfo, SerializeOptions);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _networkServerProvider.HttpClient.PostAsync("RegisterCustomer", data);
            
        _logger.LogInformation($"Response from RegisterCustomer: {response.StatusCode}");
            
        return response;
    }
    
    public async Task<AccountIdInfo?> GetAccountIdAsync(string email)
    {
        var testResponse = await _networkServerProvider.HttpClient.GetAsync($"GetAccountId?email={email}");
        var responseJson = await testResponse.Content.ReadAsStringAsync();

        var accountArray = JsonSerializer.Deserialize<AccountIdInfo>(responseJson, SerializeOptions);

        _logger.LogInformation($"Response from GetAccountId: {responseJson}");

        return accountArray;
    }

    public async Task<decimal?> GetAccountBalanceAsync(string accountId)
    {
        var response = await _networkServerProvider.HttpClient.GetAsync($"GetAccountBalance?accountId={accountId}");
        _logger.LogInformation($"Response from GetAccountBalance: {response.StatusCode}");
            
        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();
            var balanceInfo = JsonSerializer.Deserialize<BalanceInfo>(json, SerializeOptions);
            _logger.LogInformation($"Balance: {balanceInfo?.Balance}");
            return balanceInfo?.Balance;
        }
        return null;
    }
        
    public async Task<IList<AccountTransactionResponse>?> GetAccountTransactionHistory(string accountId)
    {
        var responseHistory = await _networkServerProvider.HttpClient.GetAsync($"GetAccountTransactionHistory?accountId={accountId}");
        var transactionHistory = JsonSerializer.Deserialize<AccountTransactionResponse[]>(await responseHistory.Content.ReadAsStringAsync(), SerializeOptions);

        _logger.LogInformation($"Response from GetAccountTransactionHistory: {responseHistory.StatusCode}");
        return transactionHistory;
    }

    private async Task<HttpResponseMessage?> MakeTransactionAsync(string accountId, decimal amount, string method)
    {
        await _signalRWrapper.StartSignalRAsync();
            
        var accountTransactionInfo = new AccountTransactionInfo()
        {
            CallerId = "Teller1",
            RequestId = Guid.NewGuid().ToString(),
            SchemaVersion = "1.0",
            AccountId = accountId,
            Amount = amount
        };

        var json = JsonSerializer.Serialize(accountTransactionInfo, SerializeOptions);
        var data = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await _networkServerProvider.HttpClient.PostAsync(method, data);
        return response;
    }
        
    public async Task DepositAsync(string accountId, decimal amount)
    {
        var response = await MakeTransactionAsync(accountId, amount, "Deposit");
        _logger.LogInformation($"Response from Deposit: {response?.StatusCode}");
    }

    public async Task<HttpResponseMessage?> WithdrawAsync(string accountId, decimal amount)
    {
        var response = await MakeTransactionAsync(accountId, amount, "Withdraw");
        _logger.LogInformation($"Response from Withdraw: {response?.StatusCode}");
        return response;
    }
}