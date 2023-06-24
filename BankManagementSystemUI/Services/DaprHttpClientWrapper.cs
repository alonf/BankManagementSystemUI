namespace BankManagementSystemUI.Services;

public class DaprHttpClientWrapper
{
    public readonly HttpClient HttpClient;

    public DaprHttpClientWrapper(HttpClient httpClient)
    {
        HttpClient = httpClient;
    }
}