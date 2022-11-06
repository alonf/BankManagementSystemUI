
using BankManagementSystemUI.Data;

namespace BankManagementSystemUI.Services;

public interface ISignalRWrapper
{
    Task StartSignalRAsync();
    Task StopSignalRAsync();
    event EventHandler<AccountCallbackRequest> OnSignalREvent;
}