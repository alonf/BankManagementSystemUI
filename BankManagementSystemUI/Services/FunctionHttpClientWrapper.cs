namespace BankManagementSystemUI.Services
{
    public class FunctionHttpClientWrapper
    {
        public readonly HttpClient HttpClient;

        public FunctionHttpClientWrapper(HttpClient httpClient)
        {
            HttpClient = httpClient;
        }
    }
}
