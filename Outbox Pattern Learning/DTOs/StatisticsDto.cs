namespace Outbox_Pattern_Learning.DTOs
{
    public sealed  class StatisticsDto
    {
        public long TotalMessages { get; set; } // Total number of Outbox messages.

        public long PendingMessages { get; set; } // Number of pending messages.

        public long LockedMessages { get; set; } // Number of locked messages currently being processed.

        public long PublishedMessages { get; set; } // Number of successfully published messages.

        public long FailedMessages { get; set; } // Number of failed messages waiting for retry.

        public long DeadLetterMessages { get; set; } // Number of dead-letter messages.
 
        public long TotalRetryAttempts { get; set; } // Total retry attempts across all Outbox records.

        public double AverageRetryCount { get; set; } // Average retry count per message.

        public double SuccessRate { get; set; } // Success rate as a percentage.

        public double FailureRate { get; set; } // Failure rate as a percentage.

        public DateTime GeneratedAt { get; set; }
    }
}

/*
A monitoring dashboard can display:

──────────────────────────────

Total Messages      10000

Pending             50

Locked              10

Published           9880

Failed              40

Dead Letter         20

Success Rate        98.8%

Failure Rate        0.6%

──────────────────────────────
 */