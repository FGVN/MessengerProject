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

builder.Services.AddScoped<RegisterUserCommandHandler>();
builder.Services.AddScoped<LoginUserCommandHandler>();
builder.Services.AddScoped<UserQueryHandler>();

builder.Services.AddScoped<CreateChatCommandHandler>();
builder.Services.AddScoped<DeleteChatCommandHandler>();
builder.Services.AddScoped<ChatMessageQueryHandler>();

builder.Services.AddScoped<SendMessageCommandHandler>();
builder.Services.AddScoped<DeleteMessageCommandHandler>();
builder.Services.AddScoped<EditMessageCommandHandler>();
builder.Services.AddScoped<UserChatQueryHandler>();

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

