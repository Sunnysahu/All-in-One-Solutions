namespace Outbox_Pattern_Learning.Models
{
    public enum OutboxStatus
    {
        Pending = 1,

        Locked = 2,

        Published = 3,

        Failed = 4,

        DeadLetter = 5
    }
}

/*
Outbox lifecycle
POST /api/order

        │
        ▼

Insert Order

        │
        ▼

Insert OutboxMessage

Status = Pending

        │
        ▼

Background Worker

        │
        ▼

Acquire Lock

Status = Locked

        │
        ▼

Publish to RabbitMQ

        │
 ┌──────┴────────┐
 │               │
 │ Success       │ Failure
 │               │
 ▼               ▼

Published      Failed

                    │
                    ▼

          Retry after delay

                    │
         RetryCount >= Max?

               │
         ┌─────┴──────┐
         │            │
         │ No         │ Yes
         │            │
         ▼            ▼

      Pending     DeadLetter
 */