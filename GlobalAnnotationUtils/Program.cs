using GlobalAnnotationUtils.Repositories;
using GlobalAnnotationUtils.Services;
using Microsoft.AspNetCore.Http.Timeouts;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddRequestTimeouts(options =>
//{
//    // Timeout configuration using the default policy for all endpoints
//    options.DefaultPolicy = new RequestTimeoutPolicy
//    {
//        Timeout = TimeSpan.FromSeconds(2),
//        TimeoutStatusCode = StatusCodes.Status408RequestTimeout
//    };

//    // Timeout configuration using a named policy for specific endpoints
//    options.AddPolicy("ShortTimeout", new RequestTimeoutPolicy
//    {
//        Timeout = TimeSpan.FromSeconds(5),
//        TimeoutStatusCode = StatusCodes.Status408RequestTimeout
//    });
//});

// Minimal configuration for Timeouts without using policies
builder.Services.AddRequestTimeouts();


builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IUserService, UserService>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();   
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRequestTimeouts();

app.UseAuthorization();

app.MapControllers();

app.Run();
