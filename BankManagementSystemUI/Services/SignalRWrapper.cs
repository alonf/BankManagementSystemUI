using BankManagementSystemUI.Data;
using Microsoft.AspNetCore.SignalR.Client;
using System.Net.Http;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace BankManagementSystemUI.Services;

public class SignalRWrapper : ISignalRWrapper
{
    private readonly INetworkServerProvider _networkServerProvider;
    private JsonSerializerOptions SerializeOptions => new JsonSerializerOptions
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = true
    };
    
    private HubConnection SignalRHubConnection => 
            _networkServerProvider.CurrentServerType == ServerType.Dapr ?
                _signalRDaprHubConnection :
                _signalRFunctionHubConnection;
        
    private readonly HubConnection _signalRDaprHubConnection;
    private readonly HubConnection _signalRFunctionHubConnection;
    private readonly ILogger _logger;

    public event EventHandler<AccountCallbackRequest>? OnSignalREvent;

    public SignalRWrapper(INetworkServerProvider networkServerProvider, IConfiguration configuration, ILogger<SignalRWrapper> logger)
    {
        _networkServerProvider = networkServerProvider;
        
        _logger = logger;

        _signalRDaprHubConnection = new HubConnectionBuilder()
            .WithUrl(networkServerProvider.GetSignalRNegotiationAddress(ServerType.Dapr), c=>c.Headers.Add("x-application-user-id", "Teller1"))
            .WithAutomaticReconnect()
        .Build();

        var functionKey = configuration["BMS_SIGNALR_NEGOTIATE_KEY"];
        
        _signalRFunctionHubConnection = new HubConnectionBuilder()
            .WithUrl(networkServerProvider.GetSignalRNegotiationAddress(ServerType.Function), c =>
            {
                c.Headers.Add("x-application-user-id", "Teller1");
                c.Headers.Add("x-functions-key", functionKey);
            })
            .WithAutomaticReconnect()
            .Build();
    }

    async Task ISignalRWrapper.StartSignalRAsync()
    {
        try
        {
            if (SignalRHubConnection.State == HubConnectionState.Connected)
                return;

            await SignalRHubConnection.StartAsync();

            SignalRHubConnection.On<JsonObject>("accountcallback", data =>
            {
                if (data.ContainsKey("text"))
                {
                    var argument = data["text"]!.ToString();
                    var accountCallbackRequest = JsonSerializer.Deserialize<AccountCallbackRequest>(argument, SerializeOptions);
                    OnSignalREvent?.Invoke(this, accountCallbackRequest!);
                }
                else if (data.ContainsKey("ActionName"))
                {
                    var accountCallbackRequest = JsonSerializer.Deserialize<AccountCallbackRequest>(data.ToString());
                    _logger.LogInformation($"SignalR message: {accountCallbackRequest}");
                    OnSignalREvent?.Invoke(this, accountCallbackRequest!);
                }
            });

            SignalRHubConnection.On<AccountCallbackRequest>("accountcallback", message =>
            {
                _logger.LogInformation($"Received SignalR message: {message}");
                OnSignalREvent?.Invoke(this, message!);
            });
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error starting SignalR connection");
            throw;
        }
    }

    async Task ISignalRWrapper.StopSignalRAsync()
    {
        if (SignalRHubConnection.State != HubConnectionState.Connected)
            return;

        await SignalRHubConnection.StopAsync();
    }
}