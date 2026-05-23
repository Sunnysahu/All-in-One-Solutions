using MassTransit;
using MassTransit_Setup;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    //busConfigurator.AddConsumer<CurrentTimeConsumer>();
    //busConfigurator.AddConsumer<CurrentTimeConsumerV2>();
    busConfigurator.AddConsumers(typeof(Program).Assembly);
    busConfigurator.UsingInMemory((context, config) => config.ConfigureEndpoints(context));
});

builder.Services.AddHostedService<MessagePublisher>();

var app = builder.Build();

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
