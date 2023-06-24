namespace BankManagementSystemUI.Services;

public interface INetworkServerProvider
{
    HttpClient HttpClient { get; }
    ServerType CurrentServerType { get; set; }
    string? GetSignalRNegotiationAddress(ServerType serverType);
}