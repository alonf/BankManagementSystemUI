using BankManagementSystemUI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

var accountManagerDaprUrl = Environment.GetEnvironmentVariable("BMSD_ACCOUNT_MANAGER_URL");
if (string.IsNullOrEmpty(accountManagerDaprUrl))
    accountManagerDaprUrl = "http://localhost:3500/v1.0/invoke/accountmanager/method/";
builder.Services.AddRobustHttpClient<DaprHttpClientWrapper>(baseUrl: accountManagerDaprUrl);

var accountManagerFunctionUrl = Environment.GetEnvironmentVariable("BMS_ACCOUNT_MANAGER_URL");
if (string.IsNullOrEmpty(accountManagerFunctionUrl))
    accountManagerFunctionUrl = "http://localhost:7071/api/";
builder.Services.AddRobustHttpClient<FunctionHttpClientWrapper>(baseUrl: accountManagerFunctionUrl);

builder.Services.AddSingleton<INetworkServerProvider, NetworkServerProvider>();
builder.Services.AddSingleton<IAccountManagerClient, AccountManagerClient>();
builder.Services.AddSingleton<ISignalRWrapper, SignalRWrapper>();
builder.Services.AddSingleton<ICurrentAccountHolder, CurrentAccountHolder>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
