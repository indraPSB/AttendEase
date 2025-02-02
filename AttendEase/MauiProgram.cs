using AttendEase.Services;
using AttendEase.Shared.Providers;
using AttendEase.Shared.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AttendEase
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            builder.Configuration.AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            builder.Services.AddAuthorizationCore();

            // Add device-specific services used by the AttendEase.Shared project
            builder.Services.AddSingleton<IFormFactor, FormFactor>();

            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<AuthenticationStateProvider, AttendEaseAuthenticationStateProvider>();

            builder.Services.AddScoped(sp =>
            {
                string? apiBaseUrl = builder.Configuration["ApiBaseUrl"];
                return new HttpClient
                {
                    BaseAddress = new Uri(apiBaseUrl ?? sp.GetRequiredService<NavigationManager>().BaseUri)
                };
            });

            builder.Services.AddBlazorBootstrap();

            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
