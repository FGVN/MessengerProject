﻿using Microsoft.AspNetCore.Components.WebView.Maui;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MessengerApp.Services;
using System;
using System.Net.Http;
using Microsoft.JSInterop;
using Microsoft.AspNetCore.Components;
using MatBlazor;

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

            // Register HttpClient with a base address
            builder.Services.AddScoped(sp =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
            });

            // Register HttpWrapper
            builder.Services.AddScoped<HttpWrapper>();

            // Register AuthStateProvider
            builder.Services.TryAddScoped<AuthStateProvider>();
            builder.Services.AddScoped<AuthenticationStateProvider, AuthStateProvider>();

            builder.Services.AddScoped<AuthService>();

            // Register command/query handlers
            builder.Services.AddScoped<RegisterUserCommandHandler>();
            builder.Services.AddScoped<LogoutUserCommandHandler>();
            builder.Services.AddScoped<LoginUserCommandHandler>();
            builder.Services.AddScoped<FindUsersQueryHandler>();

            // Register MatBlazor services
            builder.Services.AddMatBlazor();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
#endif

            return builder.Build();
        }
    }
}
