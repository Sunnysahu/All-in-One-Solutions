using JWT.Data;
using JWT.Repository;
using JWT.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme; // DefaultAuthenticateScheme : Tells.NET to use JWT for authentication

    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;// DefaultChallengeScheme : If unauthorized -> use JWT challenge

}).AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false; // Production → true || Local testing → false
    options.SaveToken = true; // Stores token inside HttpContext
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true, // Check token created by correct server
        ValidateAudience = true, // Check token is for correct users
        ValidateLifetime = true, // Check token not expired
        ValidateIssuerSigningKey = true, // Check token not modified

        ValidIssuer = builder.Configuration["Jwt:Issuer"], // Who created token
        ValidAudience = builder.Configuration["Jwt:Audience"], // Who can use token

        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)) // Secret key used to sign token

    };
});

builder.Services.AddAuthorization();

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
