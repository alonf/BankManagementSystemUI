﻿namespace BankManagementSystemUI.Services;

public class NetworkServerProvider : INetworkServerProvider
{
    private readonly HttpClient _daprHttpClient;
    private readonly HttpClient _functionHttpClient;
    private readonly string? _signalRDaprUrl;
    private readonly string? _signalRFunctionUrl;

    public ServerType CurrentServerType { get; set; }

    public NetworkServerProvider(DaprHttpClientWrapper daprHttpClient, FunctionHttpClientWrapper functionHttpClient, 
        IConfiguration configuration)
    {
        _daprHttpClient = daprHttpClient.HttpClient;
        _functionHttpClient = functionHttpClient.HttpClient;

        _signalRDaprUrl = configuration["BMSD_SIGNALR_URL"];
        if (string.IsNullOrEmpty(_signalRDaprUrl))
        {
            _signalRDaprUrl = "http://localhost:3502/";
        }
            
        _signalRFunctionUrl = configuration["BMS_SIGNALR_URL"];
        if (string.IsNullOrEmpty(_signalRFunctionUrl))
        {
            _signalRFunctionUrl = "http://localhost:7043/api/";
        }

        var accountManagerFunctionKey = configuration["BMS_ACCOUNT_MANAGER_KEY"];
        if (!string.IsNullOrEmpty(accountManagerFunctionKey))
        {
            functionHttpClient.HttpClient.DefaultRequestHeaders.Add("x-functions-key", accountManagerFunctionKey);
        }
    }

    public HttpClient HttpClient =>
        CurrentServerType == ServerType.Dapr ? _daprHttpClient : _functionHttpClient;
        

    public string? GetSignalRNegotiationAddress(ServerType serverType)
    {
        return serverType == ServerType.Dapr ? _signalRDaprUrl : _signalRFunctionUrl;
    }
}