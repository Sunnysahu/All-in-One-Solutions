using Hangfire;
using Microsoft.EntityFrameworkCore;
using Web_Hook.BackgroundServices;
using Web_Hook.Data;
using Web_Hook.Services;
using Web_Hook.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddHangfire(config => config.UseSqlServerStorage(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddHangfireServer(options =>
{
    //options.WorkerCount = 50;
});

builder.Services.AddScoped<IWebhookService, WebhookService>();
builder.Services.AddSingleton<IWebhookQueue, WebhookQueue>();
builder.Services.AddScoped<ISignatureService, SignatureService>();
builder.Services.AddHostedService<WebhookProcessorBackgroundService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHangfireDashboard();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();