using Microsoft.EntityFrameworkCore;
using OutBox_Pattern_with_All.Data;
using OutBox_Pattern_with_All.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddDbContext<AppDbContext>(options =>
//{
//    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
//});

builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

builder.Services.AddSingleton<RabbitMqConnection>();

//builder.Services.AddSingleton<RabbitMqInitializer>();

builder.Services.AddSingleton<RabbitMqPublisher>();

builder.Services.AddSingleton<MessageProcessor>();

builder.Services.AddHostedService<OutboxProcessorService>();

builder.Services.AddSingleton<RabbitMqTopology>();

builder.Services.AddHostedService<RabbitMqConsumer>();

var app = builder.Build();

app.Services.GetRequiredService<RabbitMqTopology>();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
