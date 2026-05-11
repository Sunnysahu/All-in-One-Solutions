using EasyNetQ;
using Messages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

var services = new ServiceCollection();

services.AddLogging(b => b.AddConsole());

services.AddEasyNetQ("host=localhost;timeout=60").UseSystemTextJson();

using var provider = services.BuildServiceProvider();

var bus = provider.GetRequiredService<IBus>();

var input = string.Empty;

Console.WriteLine("Enter a Message. 'Quit' to quit. ");
while ((input = Console.ReadLine()) != "Quit")
{
    await bus.PubSub.PublishAsync(new Textmessage { Text = input});
    Console.WriteLine("Message Published");
}