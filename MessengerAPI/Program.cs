using MessengerInfrastructure;
using Microsoft.EntityFrameworkCore;
using MessengerInfrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MessengerInfrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using DataDomain.Users;
using DataDomain.Repositories;
using Microsoft.OpenApi.Models;
using MessengerInfrastructure.CommandHandlers;
using System.Reflection;
using MessengerInfrastructure.Services.QueryHandlers;
using DataAccess.Models.Users;
using MediatR;
using MessengerDataAccess.Models.Chats;
using MessengerDataAccess.Models.Messages;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddJsonFile("appsettings.json");

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

builder.Services.AddDbContext<MessengerDbContext>(options =>
	options.UseSqlServer(connectionString));

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


builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();
builder.Services.AddScoped<IUserCommandRepository, UserCommandRepository>();

builder.Services.AddScoped<IChatMessageQueryRepository, ChatMessageQueryRepository>();
builder.Services.AddScoped<IChatMessageCommandRepository, ChatMessageCommandRepository>();

builder.Services.AddScoped<IUserChatQueryRepository, UserChatQueryRepository>();
builder.Services.AddScoped<IUserChatCommandRepository, UserChatCommandRepository>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>(); 

builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>(); 


builder.Services.Configure<JwtTokenOptions>(builder.Configuration.GetSection("JwtTokenOptions"));

builder.Services.AddTransient<IRequestHandler<GetAllUsersQuery, IEnumerable<UserMenuItemDTO>>, GetAllUsersQueryHandler>();
builder.Services.AddTransient<IRequestHandler<GetUserByIdQuery, UserMenuItemDTO>, GetUserByIdQueryHandler>();
builder.Services.AddTransient<IRequestHandler<RegisterUserDTO, string>, RegisterUserCommandHandler>();
builder.Services.AddTransient<IRequestHandler<LoginUserDTO, string>, LoginUserQueryHandler>();
builder.Services.AddTransient<IRequestHandler<SearchUsersQuery, IEnumerable<object>>, SearchUsersQueryHandler>();

builder.Services.AddScoped<RegisterUserCommandHandler>();
builder.Services.AddScoped<LoginUserQueryHandler>();
builder.Services.AddScoped<GetAllUsersQueryHandler>();
builder.Services.AddScoped<GetUserByIdQueryHandler>();
builder.Services.AddScoped<SearchUsersQueryHandler>(); 

builder.Services.AddTransient<IRequestHandler<GetAllUserChatsQuery, IEnumerable<UserChatDTO>>, GetAllUserChatsQueryHandler>();
builder.Services.AddTransient<IRequestHandler<CreateChatCommand, Guid>, CreateChatCommandHandler>();
builder.Services.AddTransient<IRequestHandler<DeleteChatCommand, bool>, DeleteChatCommandHandler>();
builder.Services.AddTransient<IRequestHandler<CreateGroupChatCommand, Guid>, CreateGroupChatCommandHandler>();
builder.Services.AddTransient<IRequestHandler<JoinGroupChatCommand>, JoinGroupChatCommandHandler>();
builder.Services.AddTransient<IRequestHandler<LeaveGroupChatCommand>, LeaveGroupChatCommandHandler>();
builder.Services.AddTransient<IRequestHandler<SearchQuery<UserChatDTO>, IEnumerable<object>>, SearchChatQueryHandler>();

builder.Services.AddScoped<SearchChatQueryHandler>();
builder.Services.AddScoped<GetAllUserChatsQueryHandler>();
builder.Services.AddScoped<CreateChatCommandHandler>();
builder.Services.AddScoped<DeleteChatCommandHandler>();
builder.Services.AddScoped<CreateGroupChatCommandHandler>();
builder.Services.AddScoped<JoinGroupChatCommandHandler>();
builder.Services.AddScoped<LeaveGroupChatCommandHandler>();


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

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.Run();

