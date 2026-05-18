using Web_Socket.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//builder.Services.AddScoped<IWebSocketRepository, WebSocketRepository>();
builder.Services.AddSingleton<IWebSocketRepository, WebSocketRepository>();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseWebSockets(); // Enables WebSocket middleware.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
