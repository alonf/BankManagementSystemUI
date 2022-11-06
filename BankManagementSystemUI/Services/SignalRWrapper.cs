using BankManagementSystemUI.Data;
using Microsoft.AspNetCore.SignalR.Client;

namespace BankManagementSystemUI.Services;

public class SignalRWrapper : ISignalRWrapper
{
    private readonly INetworkServerProvider _networkServerProvider;
    private HubConnection SignalRHubConnection => 
            _networkServerProvider.CurrentServerType == ServerType.Dapr ?
                _signalRDaprHubConnection :
                _signalRFunctionHubConnection;
        
    private readonly HubConnection _signalRDaprHubConnection;
    private readonly HubConnection _signalRFunctionHubConnection;
    private readonly ILogger _logger;

    public event EventHandler<AccountCallbackRequest>? OnSignalREvent;

    public SignalRWrapper(INetworkServerProvider networkServerProvider, ILogger<SignalRWrapper> logger, IAccountManagerClient accountManagerClient)
    {
        _networkServerProvider = networkServerProvider;
        
        _logger = logger;

        _signalRDaprHubConnection = new HubConnectionBuilder()
            .WithUrl(networkServerProvider.GetSignalRNegotiationAddress(ServerType.Dapr), c=>c.Headers.Add("x-application-user-id", "Teller1"))
            .WithAutomaticReconnect()
            .Build();

        _signalRFunctionHubConnection = new HubConnectionBuilder()
            .WithUrl(networkServerProvider.GetSignalRNegotiationAddress(ServerType.Function), c => c.Headers.Add("x-application-user-id", "Teller1"))
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

            SignalRHubConnection.On<Argument>("accountcallback", message =>
            {
                _logger.LogInformation($"Received SignalR message: {message.Text}");
                OnSignalREvent?.Invoke(this, message.Text!);
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