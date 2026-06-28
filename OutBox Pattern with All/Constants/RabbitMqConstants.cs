    namespace OutBox_Pattern_with_All.Constants
{
    public static class RabbitMqConstants
    {
        // Exchange
        public const string Exchange = "order.exchange";

        // Main Queue
        public const string Queue = "order.queue";

        // Retry Queue
        public const string RetryQueue = "order.queue.retry";

        // Dead Letter Queue
        public const string DeadLetterQueue = "order.queue.dlq";

        // Routing Keys
        public const string RoutingKey = "order.created";

        public const string RetryRoutingKey = "order.created.retry";

        public const string DeadLetterRoutingKey = "order.created.dlq";

        // Retry Delay (30 seconds)
        public const int RetryDelayMilliseconds = 30000;

        // Maximum retries before moving to DLQ
        public const int MaxRetryCount = 5;
    }
}
