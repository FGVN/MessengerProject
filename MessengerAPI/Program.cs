using MessengerInfrastructure;
using Microsoft.EntityFrameworkCore;
using MessengerInfrastructure.Services;
using DataDomain.Repositories;
using MessengerInfrastructure.Services.InterFaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

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
builder.Services.AddSwaggerGen();

// Register repositories
builder.Services.AddScoped<IUserCommandRepository, UserRepository>();
builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();

// Register Unit of Work
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Register services
builder.Services.AddScoped<IUserCommand, UserCommand>();
builder.Services.AddScoped<RegisterUserCommandHandler>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
	app.UseSwagger();
	app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

app.Run();
