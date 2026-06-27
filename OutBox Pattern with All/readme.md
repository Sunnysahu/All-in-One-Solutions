```
OutBox_Pattern_with_All
│
├── Constants
│   ├── DbConstants.cs
│   ├── RabbitMqConstants.cs
│
├── Controllers
│   └── OrderController.cs
│
├── Data
│   └── AppDbContext.cs
│
├── Entities
│   ├── Order.cs
│   ├── OutboxMessage.cs
│   ├── ProcessedMessage.cs          <-- NEW
│
├── Models
│   ├── CreateOrderRequest.cs
│   └── OrderCreatedEvent.cs
│
├── Services
│   ├── RabbitMqConnection.cs
│   ├── RabbitMqInitializer.cs
│   ├── RabbitMqPublisher.cs
│   ├── RabbitMqConsumer.cs          <-- NEW
│   ├── OutboxProcessorService.cs
│   └── MessageProcessor.cs          <-- NEW
│
├── Program.cs
└── appsettings.json
```

## FLOW

```
Client
   |
   V

Create Order API

   |
   V

SQL Transaction Start

   |
   +---- Insert Order
   |
   +---- Insert Outbox Record

SQL Commit

   |
   V

BackgroundService

   |
   V

Read Pending Outbox

   |
   V

Publish To RabbitMQ

   |
   V

Mark Processed
```

## If application crashes:

```
Order Saved
Outbox Saved
Application Crash

Restart

BackgroundService Starts

Reads Pending Rows

Publishes Again

Done

No event loss
```


UNDERSTANDING

DECLARE THE CONNECTION AND CHANNEL, THEN CREATE QUEUE, EXCHANGE, BINDING
```
 public sealed class RabbitMqInitializer : IAsyncDisposable
    {
        public IChannel Channel { get; }

        public RabbitMqInitializer(RabbitMqConnection connection)
        {
            Channel = connection.Connection.CreateChannelAsync().GetAwaiter().GetResult();

            Channel.ExchangeDeclareAsync(
                exchange: RabbitMqConstants.Exchange,
                type: ExchangeType.Direct,
                durable: true
            ).GetAwaiter().GetResult();

            Channel.QueueDeclareAsync(
                queue: RabbitMqConstants.Queue,
                durable: true,
                exclusive: false,
                autoDelete: false
            ).GetAwaiter().GetResult();

            Channel.QueueBindAsync(
                queue: RabbitMqConstants.Queue,
                exchange: RabbitMqConstants.Exchange,
                routingKey: RabbitMqConstants.RoutingKey
            ).GetAwaiter().GetResult();
        }

        public async ValueTask DisposeAsync()
        {
            await Channel.DisposeAsync();
        }
    }
```


PUBLISH IT FROM BACKGROUND SERVICE

```
ublic class OutboxProcessorService : BackgroundService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;
        private readonly RabbitMqPublisher _publisher;

        public OutboxProcessorService(IDbContextFactory<AppDbContext> dbFactory, RabbitMqPublisher publisher)
        {
            _dbFactory = dbFactory;
            _publisher = publisher;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                Console.WriteLine("Hello");

                await ProcessMessages(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(5),stoppingToken);
            }
        }

        private async Task ProcessMessages(CancellationToken cancellationToken)
        {
            await using var db = await _dbFactory.CreateDbContextAsync(cancellationToken);

            var messages =
                await db.OutboxMessages
                    .Where(x => x.Status == DbConstants.Pending)
                    .OrderBy(x => x.CreatedAt)
                    .Take(100)
                    .ToListAsync();

            foreach (var message in messages)
            {
                try
                {
                    await _publisher.PublishAsync(message.Payload);

                    message.Status = DbConstants.Processed;

                    message.ProcessedAt = DateTime.Now;
                }
                catch
                {
                    message.RetryCount++;
                }
            }

            await db.SaveChangesAsync(cancellationToken);
        }
    }
```

CONSUME IT 
```

```
---

TO DO THIS MANUALLY FROM RABBIT MQ UI

### Step - by  -Step flow for a 3-node cluster (leader + 2 replicas).

# RabbitMQ 3-Node Cluster with Docker

## Prerequisites

* Docker Desktop installed
* Docker Compose available

---

# 1. Create `docker-compose.yml`

```yaml
version: "3.9"

services:
  rabbit1:
    image: rabbitmq:4-management
    container_name: rabbit1
    hostname: rabbit1
    ports:
      - "5672:5672"
      - "15672:15672"
    environment:
      RABBITMQ_DEFAULT_USER: guest
      RABBITMQ_DEFAULT_PASS: guest
      RABBITMQ_ERLANG_COOKIE: rabbit-cluster-cookie

  rabbit2:
    image: rabbitmq:4-management
    container_name: rabbit2
    hostname: rabbit2
    environment:
      RABBITMQ_ERLANG_COOKIE: rabbit-cluster-cookie

  rabbit3:
    image: rabbitmq:4-management
    container_name: rabbit3
    hostname: rabbit3
    environment:
      RABBITMQ_ERLANG_COOKIE: rabbit-cluster-cookie
```

---

# 2. Start the containers

```bash
docker compose up -d
```

Verify:

```bash
docker ps
```

You should see:

* rabbit1
* rabbit2
* rabbit3

---

# 3. Create the RabbitMQ Cluster

## Join rabbit2

```bash
docker exec -it rabbit2 bash
```

```bash
rabbitmqctl stop_app
rabbitmqctl reset
rabbitmqctl join_cluster rabbit@rabbit1
rabbitmqctl start_app
```

Exit:

```bash
exit
```

---

## Join rabbit3

```bash
docker exec -it rabbit3 bash
```

```bash
rabbitmqctl stop_app
rabbitmqctl reset
rabbitmqctl join_cluster rabbit@rabbit1
rabbitmqctl start_app
```

Exit:

```bash
exit
```

---

# 4. Verify the cluster

```bash
docker exec rabbit1 rabbitmqctl cluster_status
```

Expected output includes:

```
Running Nodes

rabbit@rabbit1
rabbit@rabbit2
rabbit@rabbit3
```

---

# 5. Open RabbitMQ Management UI

Open:

```
http://localhost:15672
```

Login:

```
Username: guest
Password: guest
```

---

# 6. Create an Exchange

Navigate:

```
Exchanges
```

Click **Add exchange**

| Option      | Value          |
| ----------- | -------------- |
| Name        | order.exchange |
| Type        | direct         |
| Durable     | Yes            |
| Auto Delete | No             |
| Internal    | No             |

Click **Add exchange**.

---

# 7. Create a Queue

Navigate:

```
Queues
```

Click **Add Queue**

| Option      | Value                              |
| ----------- | ---------------------------------- |
| Name        | orders                             |
| Type        | Quorum *(or Classic if preferred)* |
| Durable     | Yes                                |
| Auto Delete | No                                 |

Click **Add Queue**.

---

# 8. Bind Queue to Exchange

Open:

```
Exchanges
→ order.exchange
```

Scroll to **Bindings**

Click **Add binding from this exchange**

| Option           | Value         |
| ---------------- | ------------- |
| Destination      | orders        |
| Destination Type | Queue         |
| Routing Key      | order.created |

Click **Bind**.

---

# 9. Publish a Message

Open:

```
Exchanges
→ order.exchange
```

Scroll to **Publish message**

Routing Key:

```
order.created
```

Payload:

```json
{
  "OrderId": 1001,
  "Customer": "Alice"
}
```

Click **Publish Message**.

---

# 10. Verify Message

Navigate:

```
Queues
→ orders
```

You should see:

```
Ready = 1
```

---

# 11. Read the Message

Still on the queue page.

Scroll to:

```
Get Messages
```

Select:

```
Ack Mode:
Ack message, requeue = false
```

Click:

```
Get Message(s)
```

The JSON message will be displayed.

---

# 12. Verify Quorum Queue Replication

If you created a **Quorum** queue:

Open:

```
Queues
→ orders
```

You should see information similar to:

```
Leader:
rabbit@rabbit1

Members:
rabbit@rabbit1
rabbit@rabbit2
rabbit@rabbit3
```

The leader handles reads and writes, while the other two nodes maintain replicas. If the leader node goes down, another member can be elected leader, allowing the queue to continue operating.
