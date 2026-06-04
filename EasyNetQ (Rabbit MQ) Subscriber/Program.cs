using EasyNetQ;
using EasyNetQ__Rabbit_MQ__Subscriber;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(b => b.AddConsole());

builder.Services
    .AddEasyNetQ("host=localhost")
    .UseSystemTextJson();


/*
If You Want OrderPlacedConsumer To Be Called

Change:

    builder.Services.AddHostedService<ManualSubscriberWorker>();

to:

    builder.Services.AddHostedService<OrderSubscriberWorker>();

and register the consumer:

    builder.Services.AddTransient<OrderPlacedComsumer>();

PROCESS : 

Application Start
      |
      v
OrderSubscriberWorker
      |
      v
AutoSubscriber
      |
      v
Scan Assembly
      |
      v
Find OrderPlacedComsumer
      |
      v
Subscribe to OrderPlacedMessage
 */

builder.Services.AddTransient<OrderPlacedComsumer>();
builder.Services.AddHostedService<ManualSubscriberWorker>();
builder.Services.AddHostedService<OrderSubscriberWorker>();

await builder.Build().RunAsync();

//var services = new ServiceCollection();

//services.AddLogging(b => b.AddConsole());

//services.AddEasyNetQ("host=localhost;timeout=60").UseSystemTextJson();

//using var provider = services.BuildServiceProvider();

//var bus = provider.GetRequiredService<IBus>();

//await bus.PubSub.SubscribeAsync<Textmessage>("Message", msg =>
//{
//    Console.WriteLine("Got Message : " + msg.Text);
//});

//Console.WriteLine("Listening for message. Hit <return> to quit.");
//Console.ReadLine();
