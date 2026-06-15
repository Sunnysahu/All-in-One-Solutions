namespace Outbox_Pattern_Learning.Constants
{
    public static class ApplicationConstants
    {
        public static class Api
        {
            public const string ApiName = "OutboxPattern.Api";

            public const string ApiVersion = "v1";

            public const string JsonContentType = "application/json";
        }

        public static class RabbitMq
        {
            public const string DefaultExchange = "application.events";

            public const string DefaultExchangeType = "topic";

            public const string DefaultQueue = "order-processing";

            public const string DefaultRoutingKey = "orders.created";
        }

        public static class Events
        {
            public const string OrderCreated = "OrderCreated";

            public const string OrderUpdated = "OrderUpdated";

            public const string OrderCancelled = "OrderCancelled";
        }

        public static class Headers
        {
            public const string CorrelationId = "X-Correlation-Id";

            public const string MessageId = "X-Message-Id";

            public const string RequestId = "X-Request-Id";
        }

        public static class RetryIntervals
        {
            public static readonly TimeSpan[] ExponentialBackoff =
            {
            TimeSpan.FromSeconds(1),
            TimeSpan.FromSeconds(2),
            TimeSpan.FromSeconds(4),
            TimeSpan.FromSeconds(8),
            TimeSpan.FromSeconds(16),
            TimeSpan.FromSeconds(32),
            TimeSpan.FromMinutes(1),
            TimeSpan.FromMinutes(5)
            };
        }

        public static class Logging
        {
            public const int WorkerStarted = 1000;

            public const int WorkerStopped = 1001;

            public const int PublishSucceeded = 1002;

            public const int PublishFailed = 1003;

            public const int RetryScheduled = 1004;

            public const int DeadLetterMoved = 1005;

            public const int DuplicateDetected = 1006;
        }

        public static class Locking
        {
            public const string WorkerPrefix = "OUTBOX-WORKER-";
        }

        public static class Errors
        {
            public const string UnexpectedError = "An unexpected error occurred.";

            public const string OrderNotFound = "Order was not found.";

            public const string DuplicateMessage = "Duplicate message detected.";

            public const string InvalidRequest = "Invalid request.";
        }
    }
}
