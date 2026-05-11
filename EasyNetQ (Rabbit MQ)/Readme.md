
.NET RabbitMQ with EasyNetQ

This project demonstrates messaging between services using **RabbitMQ** and **EasyNetQ** in .NET with two Class Library projects (Publisher & Subscriber).
```md

📦 Tech Stack

- .NET (Class Libraries)
- RabbitMQ
- EasyNetQ

GitHub: https://github.com/EasyNetQ/EasyNetQ

📁 Project Structure

```

Solution
│
├── Publisher        (Class Library - sends messages)
├── Subscriber       (Class Library - receives messages)
└── Messages   (Optional shared models)

````

---

🚀 Setup Instructions

1. Create Projects

```bash
dotnet new classlib -n Publisher
dotnet new classlib -n Subscriber
dotnet new classlib -n Messages
````

---

2. Install EasyNetQ Package

Run in both Publisher and Subscriber projects:

```bash
dotnet add package EasyNetQ
```

---

3. Run RabbitMQ using Docker

```bash
docker run -d --name rabbitmq ^
-p 5672:5672 ^
-p 15672:15672 ^
rabbitmq:3-management
```

👉 For Linux/macOS:

```bash
docker run -d --name rabbitmq \
-p 5672:5672 \
-p 15672:15672 \
rabbitmq:3-management
```

---

## 🌐 RabbitMQ Management UI

Open in browser:

[http://localhost:15672](http://localhost:15672)

### Default Login:

```
Username: guest
Password: guest
```

---

## ⚙️ Run Multiple Projects in Visual Studio

To run Publisher and Subscriber together:

1. Right-click Solution
2. Click Properties
3. Select **Multiple startup projects**
4. Set both:

   * Publisher → Start
   * Subscriber → Start
5. Click Apply → OK

---

## 🔌 Basic Connection Code

```csharp
using EasyNetQ;

var bus = RabbitHutch.CreateBus("host=localhost");
```

---

## 📩 Example Usage

### Publisher

```csharp
/*
bus.PubSub.Publish(new MyMessage
{
    Text = "Hello RabbitMQ"
});
*/

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

```

### Subscriber

```csharp
/*
bus.PubSub.Subscribe<MyMessage>("test_subscription", msg =>
{
    Console.WriteLine(msg.Text);
});
*/

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

```

---

## 📌 Notes

* Make sure Docker container is running before starting projects
* RabbitMQ uses ports:

  * 5672 → messaging
  * 15672 → UI dashboard
* Keep Publisher and Subscriber running simultaneously

---

## 📚 References

* EasyNetQ: [https://github.com/EasyNetQ/EasyNetQ](https://github.com/EasyNetQ/EasyNetQ)
* RabbitMQ: [https://www.rabbitmq.com](https://www.rabbitmq.com)
* Docker RabbitMQ Image: [https://hub.docker.com/_/rabbitmq](https://hub.docker.com/_/rabbitmq)

```
If you want, I can also upgrade this into a **full working sample solution (Publisher + Subscriber + Shared DTOs + DI setup)**.
```
