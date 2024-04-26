using MediatR;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MessengerInfrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using Microsoft.OpenApi.Models;
using System.Reflection;
using DataAccess.Models;
using MessengerInfrastructure.Hubs;
using MessengerInfrastructure.QueryHandlers;
using MessengerInfrastructure.CommandHandlers;
using DataAccess;
using DataAccess.Repositories;
using MessengerInfrastructure.Query;
using MessengerInfrastructure.Commands;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MessengerDbContext>(options =>
	options.UseSqlServer(connectionString));

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

var jwtTokenOptions = builder.Configuration.GetSection("JwtTokenOptions").Get<JwtTokenOptions>();

builder.Services.AddAuthentication(options =>
{
	options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
	options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
	options.TokenValidationParameters = new TokenValidationParameters
	{
		ValidateIssuer = true,
		ValidateAudience = true,
		ValidateLifetime = false,
		ValidateIssuerSigningKey = true,
		ValidIssuer = jwtTokenOptions.Issuer,
		ValidAudience = jwtTokenOptions.Audience,
		IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenOptions.SecretKey))
	};
});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
builder.Services.AddSwaggerGen(option =>
{
    option.SwaggerDoc("v1", new OpenApiInfo { Title = "Demo API", Version = "v1" });
    option.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please enter a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        }
    );
    option.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        }
    );
});

builder.Services.AddIdentity<User, IdentityRole>(options =>
{

})
.AddEntityFrameworkStores<MessengerDbContext>()
.AddDefaultTokenProviders();


builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IChatMessageRepository, ChatMessageRepository>();

builder.Services.AddScoped<IUserChatRepository, UserChatRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>(); 

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>(); 


builder.Services.Configure<JwtTokenOptions>(builder.Configuration.GetSection("JwtTokenOptions"));

builder.Services.AddSignalR();

// Users
builder.Services.AddTransient<IRequestHandler<GetAllUsersQuery, IEnumerable<UserMenuItemDTO>>, GetAllUsersQueryHandler>();
builder.Services.AddTransient<IRequestHandler<RegisterUserCommand, string>, RegisterUserCommandHandler>();
builder.Services.AddTransient<IRequestHandler<LoginUserQuery, string>, LoginUserQueryHandler>();
builder.Services.AddTransient<IRequestHandler<SearchQuery<UserMenuItemDTO>, IEnumerable<object>>, SearchUsersQueryHandler>();

builder.Services.AddScoped<RegisterUserCommandHandler>();
builder.Services.AddScoped<LoginUserQueryHandler>();
builder.Services.AddScoped<GetAllUsersQueryHandler>();
builder.Services.AddScoped<SearchUsersQueryHandler>(); 

// Chats
builder.Services.AddTransient<IRequestHandler<GetAllUserChatsQuery, IEnumerable<UserChatDTO>>, GetAllUserChatsQueryHandler>();
builder.Services.AddTransient<IRequestHandler<CreateChatCommand, Guid>, CreateChatCommandHandler>();
builder.Services.AddTransient<IRequestHandler<DeleteChatCommand, bool>, DeleteChatCommandHandler>();
builder.Services.AddTransient<IRequestHandler<SearchQuery<UserChatDTO>, IEnumerable<object>>, SearchChatQueryHandler>();

builder.Services.AddScoped<SearchChatQueryHandler>();
builder.Services.AddScoped<GetAllUserChatsQueryHandler>();
builder.Services.AddScoped<CreateChatCommandHandler>();
builder.Services.AddScoped<DeleteChatCommandHandler>();

// Group chats
builder.Services.AddTransient<IRequestHandler<CreateGroupChatCommand, Guid>, CreateGroupChatCommandHandler>();
builder.Services.AddTransient<IRequestHandler<JoinGroupChatCommand>, JoinGroupChatCommandHandler>();
builder.Services.AddTransient<IRequestHandler<LeaveGroupChatCommand>, LeaveGroupChatCommandHandler>();
builder.Services.AddTransient<IRequestHandler<UpdateGroupChatCommand>, UpdateGroupChatCommandHandler>();
builder.Services.AddTransient<IRequestHandler<MyGroupChatsQuery, IEnumerable<GroupChat>>, MyGroupChatsQueryHandler>();
builder.Services.AddTransient<IRequestHandler<SearchQuery<GroupChatDTO>, IEnumerable<object>>, SearchGroupChatsQueryHandler>();

builder.Services.AddScoped<CreateGroupChatCommandHandler>();
builder.Services.AddScoped<JoinGroupChatCommandHandler>();
builder.Services.AddScoped<UpdateGroupChatCommandHandler>();
builder.Services.AddScoped<LeaveGroupChatCommandHandler>();
builder.Services.AddScoped<MyGroupChatsQueryHandler>();
builder.Services.AddScoped<SearchGroupChatsQueryHandler>();

// Messages
builder.Services.AddTransient<IRequestHandler<SearchQuery<ChatMessageDTO>, IEnumerable<object>>, SearchMessageQueryHandler>();
builder.Services.AddTransient<IRequestHandler<SendMessageCommand, int>, SendMessageCommandHandler>();
builder.Services.AddTransient<IRequestHandler<DeleteMessageCommand, Unit>, DeleteMessageCommandHandler>();
builder.Services.AddTransient<IRequestHandler<EditMessageCommand, Unit>, EditMessageCommandHandler>();
builder.Services.AddTransient<IRequestHandler<DeleteMessageCommand, Unit>, DeleteMessageCommandHandler>();

builder.Services.AddScoped<SearchMessageQueryHandler>();
builder.Services.AddScoped<SendMessageCommandHandler>();
builder.Services.AddScoped<DeleteMessageCommandHandler>();
builder.Services.AddScoped<EditMessageCommandHandler>();
builder.Services.AddScoped<SearchChatQueryHandler>();

builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

var app = builder.Build();

app.UseCors("AllowAll");

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.MapHub<ChatHub>("/chathub");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.Run();