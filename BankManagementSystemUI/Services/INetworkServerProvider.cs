namespace BankManagementSystemUI.Services
{
    public interface INetworkServerProvider
    {
        HttpClient GetHttpClient();
        ServerType CurrentServerType { get; set; }
        string GetSignalRNegotiationAddress(ServerType serverType);
    }
}
