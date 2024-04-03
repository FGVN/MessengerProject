using MessengerInfrastructure;
using Microsoft.EntityFrameworkCore;
using MessengerInfrastructure.Services;
using DataDomain.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MessengerInfrastructure.Utilities;
using Microsoft.AspNetCore.Identity;
using DataDomain.Users;
using MessengerInfrastructure.Services.InterFaces;

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

builder.Services.AddIdentity<User, IdentityRole>(options =>
{

})
.AddEntityFrameworkStores<MessengerDbContext>()
.AddDefaultTokenProviders();

builder.Services.AddScoped<UserManager<User>>();
builder.Services.AddScoped<SignInManager<User>>(); 
builder.Services.AddScoped<JwtTokenGenerator>(); 


builder.Services.Configure<JwtTokenOptions>(builder.Configuration.GetSection("JwtTokenOptions"));



//// Register repositories
//builder.Services.AddScoped<IUserCommandRepository, UserRepository>();
//builder.Services.AddScoped<IUserQueryRepository, UserQueryRepository>();

//// Register Unit of Work
//builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

//// Register Utils
//builder.Services.AddScoped<JwtTokenGenerator>();

//// Register services
//builder.Services.AddScoped<IUserCommand, UserCommand>();
//builder.Services.AddScoped<RegisterUserCommandHandler>();

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

