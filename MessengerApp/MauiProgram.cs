using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MessengerApp.Services;
using System;
using System.Net.Http;
using Microsoft.JSInterop;

namespace MessengerApp
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			builder
				.UseMauiApp<App>()
				.ConfigureFonts(fonts =>
				{
					fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				});

			builder.Services.AddMauiBlazorWebView();
			builder.Services.AddAuthorizationCore();

            builder.Services.AddScoped<HttpClient>();

            builder.Services.TryAddScoped<AuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

			builder.Services.AddScoped<AuthService>();

			// Register HttpClient as a scoped service

			builder.Services.AddScoped<RegisterUserCommandHandler>();
			builder.Services.AddScoped<LogoutUserCommandHandler>();
			builder.Services.AddScoped<LoginUserCommandHandler>();

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
#endif

			return builder.Build();
		}
	}
}
