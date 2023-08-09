using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Identity.Web;
using Microsoft.Identity.Web.UI;
using ModelFactory.Config;
using System;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var initialScopes = builder.Configuration["DownstreamApi:Scopes"]?.Split(' ') ?? builder.Configuration["MicrosoftGraph:Scopes"]?.Split(' ');


var kv = new ClientSideKeyVaultSettings(builder.Configuration);
builder.Services.AddSingleton<ClientSideKeyVaultSettings>(kv);

builder.Services.AddMicrosoftIdentityWebAppAuthentication(builder.Configuration, "AzureB2C")
                    .EnableTokenAcquisitionToCallDownstreamApi(new string[] { "https://whosright.onmicrosoft.com/ca09d93b-6ffe-4bb5-a53b-3ddf1edeee0b/access" })
                    .AddInMemoryTokenCaches();


builder.Services.AddMudServices();

builder.Services.AddControllersWithViews()
    .AddMicrosoftIdentityUI();


builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor()
    .AddMicrosoftIdentityConsentHandler();
builder.Services.AddHttpContextAccessor();

// registers HTTP client that uses the managed user access token
builder.Services.AddUserAccessTokenHttpClient("api_client", configureClient: client =>
{
    client.BaseAddress = new Uri("https://localhost:5002/");
});

builder.Services.AddScoped<Web.TokenProvider>();
builder.Services.AddHttpClient();

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

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
