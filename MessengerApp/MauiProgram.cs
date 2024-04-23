using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection.Extensions;
using MessengerApp.Services;
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

            Uri BaseUri = new Uri("https://localhost:8956/");

            // Register HttpClient with a base address
            builder.Services.AddScoped(sp =>
            {
                var navigationManager = sp.GetRequiredService<NavigationManager>();
                return new HttpClient { BaseAddress = new Uri(navigationManager.BaseUri) };
            });

            builder.Services.AddScoped<HttpClient>(sp =>
            {
                return new HttpClient { BaseAddress = BaseUri };
            });

            builder.Services.AddScoped<HttpWrapper>(sp =>
            {
                var httpClient = sp.GetRequiredService<HttpClient>();
                return new HttpWrapper(httpClient, BaseUri + "api/");
            });


            // Register AuthStateProvider
            builder.Services.AddScoped<AuthStateProvider>();
            builder.Services.TryAddScoped<AuthenticationStateProvider, AuthStateProvider>();

            builder.Services.AddScoped<AuthService>();

            builder.Services.AddScoped<LocalStorageUtils>();

            // Register command/query handlers
            builder.Services.AddScoped<RegisterUserCommandHandler>();
            builder.Services.AddScoped<LogoutUserCommandHandler>();
            builder.Services.AddScoped<LoginUserQueryHandler>();
            builder.Services.AddScoped<FindUsersQueryHandler>();

            builder.Services.AddScoped<CreateChatCommandHandler>();
            builder.Services.AddScoped<DeleteChatCommandHandler>();
            builder.Services.AddScoped<FindChatsQueryHandler>();

            builder.Services.AddScoped<CreateGroupChatCommandHandler>();
            builder.Services.AddScoped<JoinGroupChatCommandHandler>();
            builder.Services.AddScoped<LeaveGroupChatCommandHandler>();
            builder.Services.AddScoped<UpdateGroupChatCommandHandler>();
            builder.Services.AddScoped<MyGroupChatsQueryHandler>();

            builder.Services.AddScoped<FindChatMessageQueryHandler>();



            // Register SignalR client
            builder.Services.AddSingleton<ChatClient>(sp => 
            {
                return new ChatClient(BaseUri);
            });


            // Register MatBlazor services
            builder.Services.AddMatBlazor();

            #if DEBUG
                builder.Services.AddBlazorWebViewDeveloperTools();
            #endif

            return builder.Build();
        }
    }
}
