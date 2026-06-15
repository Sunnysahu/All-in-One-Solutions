namespace Outbox_Pattern_Learning.Configuration
{
    public class OutboxOptions
    {
        public int BatchSize { get; set; } = 100; /// Maximum number of messages processed in one polling cycle.
        public int PollingIntervalSeconds { get; set; } = 5; /// Time interval (in seconds) between worker polling cycles.
        public int MaximumRetryCount { get; set; } = 8; /// Duration (in seconds) for which a message remains locked by a worker.
        public int LockDurationSeconds { get; set; } = 60; /// Maximum retry attempts before moving a message to DeadLetter status.
        public bool DeadLetterEnabled { get; set; } = true; /// Status name assigned when a message is permanently failed.

        public int MaxFetchSize { get; set; } = 500;

        public bool EnableLockRecovery { get; set; } = true;

        public int LockExpirationSeconds { get; set; } = 60;

        public bool EnableExponentialBackoff { get; set; } = true;

        public bool EnableLogging { get; set; } = true;
    }
}

/*
Interaction with the project

appsettings.json
        │
        ▼
OutboxOptions
        │
        ▼
Dependency Injection
        │
        ▼
OutboxPublisherBackgroundService
        │
        ▼
Worker Logic
        │
 ├──────── Batch Size
 ├──────── Poll Interval
 ├──────── Lock Duration
 ├──────── Retry Count
 └──────── Dead Letter Policy
 */