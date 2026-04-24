using Microsoft.EntityFrameworkCore;
using Redis.Caching;
using Redis.Data;
using Redis.Repository;
using Redis.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("Default"));
});

builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IRedisCacheService, RedisCacheService>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    var connection = builder.Configuration["Redis:Connection"];
    var user = builder.Configuration["Redis:User"];
    var password = builder.Configuration["Redis:Password"];

    options.Configuration = $"{connection},user={user},password={password}";
    options.InstanceName = builder.Configuration["Redis:InstanceName"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
        c.InjectStylesheet("/swagger-dark.css");
    });
}

app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
