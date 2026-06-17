```
OutboxPatternDemo
│
├── Controllers
│     └── OrderController.cs
│
├── Services
│     ├── OrderService.cs
│     ├── OutboxService.cs
│     └── MessagePublisher.cs
│
├── BackgroundServices
│     └── OutboxWorker.cs
│
├── Models
│     ├── Order.cs
│     ├── OutboxMessage.cs
│     ├── DeadLetterMessage.cs
│     └── ProcessedMessage.cs
│
├── DTOs
│     └── CreateOrderRequest.cs
│
├── Data
│     └── AppDbContext.cs
│
├── SQL
│     ├──001_CreateOrders.sql
│     ├──002_CreateOutbox.sql
│     ├──003_CreateDeadLetter.sql
│     ├──004_CreateProcessedMessages.sql
│     └──005_CreateIndexes.sql
│
├── Program.cs
│
└── appsettings.json
```