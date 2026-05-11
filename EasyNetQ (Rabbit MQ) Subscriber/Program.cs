using EasyNetQ;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.ComponentModel;

var services = new ServiceCollection();

services.AddLogging(b => b.AddConsole());

services.AddEasyNetQ("host=localhost;timeout=60").UseSystemTextJson();

using var provider = services.BuildServiceProvider();

var bus = provider.GetRequiredService<IBus>();

await bus.PubSub.SubscribeAsync<Textmessage>("Message", msg =>
{
    Console.WriteLine("Got Message : " + msg.Text);
});

Console.WriteLine("Listening for message. Hit <return> to quit.");
Console.ReadLine();
