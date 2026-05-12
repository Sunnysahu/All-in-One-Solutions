using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Publisher;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddLogging(b => b.AddConsole());

builder.Services
    .AddEasyNetQ("host=localhost")
    .UseSystemTextJson();

builder.Services.AddHostedService<OrderPublisherWorker>();

await builder.Build().RunAsync();


//await bus.PubSub.SubscribeAsync<Textmessage>(
//    "text_subscriber", 
//    async msg =>
//    {
//        Console.WriteLine(msg.Text);
//});
//var input = string.Empty;

//Console.WriteLine("Enter a Message. 'Quit' to quit. ");
//while ((input = Console.ReadLine()) != "Quit")
//{
//    await bus.PubSub.PublishAsync(new Textmessage { Text = input });
//    Console.WriteLine("Message Published");
//}