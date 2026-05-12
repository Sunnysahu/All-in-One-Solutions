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

builder.Services.AddHostedService<ManualSubscriberWorker>();

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
