using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MessengerApp.Services;
using System;
using System.Net.Http;

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
			builder.Services.AddScoped<AuthenticatedUser>();
            builder.Services.AddSingleton<AuthStateProvider>();
            builder.Services.TryAddScoped<AuthenticationStateProvider, AuthStateProvider>();
			builder.Services.AddSingleton<AuthService>();

			// Register HttpClient as a scoped service
			builder.Services.AddScoped<HttpClient>(sp =>
			{
				var httpClient = new HttpClient
				{
				};
				return httpClient;
			});

			builder.Services.AddScoped<RegisterUserCommandHandler>();

#if DEBUG
			builder.Services.AddBlazorWebViewDeveloperTools();
#endif

			return builder.Build();
		}
	}
}
