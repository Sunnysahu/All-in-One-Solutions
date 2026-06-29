using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using OutBox_Pattern_with_All.Constants;
using OutBox_Pattern_with_All.Data;
using OutBox_Pattern_with_All.Entities;
using System.Data;

namespace OutBox_Pattern_with_All.Services
{
    public class OutboxMessageLeaseService
    {
        private readonly IDbContextFactory<AppDbContext> _dbFactory;

        public OutboxMessageLeaseService(IDbContextFactory<AppDbContext> dbFactory)
        {
            _dbFactory = dbFactory;
        }

        public async Task<List<OutboxMessage>> AcquireMessagesAsync()
        {
            var messages = new List<OutboxMessage>();

            await using var db = await _dbFactory.CreateDbContextAsync();

            await using var connection = (SqlConnection) db.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open)
            {
                await connection.OpenAsync();
            }

            await using var transaction = await connection.BeginTransactionAsync();

            var command = connection.CreateCommand();

            command.Transaction = (SqlTransaction) transaction;

            command.CommandText =
            """
            ;WITH MessagesToLock AS
            (
                SELECT TOP (@BatchSize) *
                FROM OutboxMessages WITH (UPDLOCK, READPAST, ROWLOCK)

                WHERE Status = @Pending

                AND
                (
                    LockedBy IS NULL

                    OR

                    LockedAt < DATEADD
                    (
                        MINUTE,
                        -@LockTimeout,
                        SYSUTCDATETIME()
                    )
                )

                ORDER BY CreatedAt
            )

            UPDATE MessagesToLock

            SET

                LockedBy = @WorkerId,

                LockedAt = SYSUTCDATETIME()

            OUTPUT INSERTED.*;
            """;

            command.Parameters.AddWithValue("@BatchSize", WorkerConstants.BatchSize);

            command.Parameters.AddWithValue("@Pending", DbConstants.Pending);

            command.Parameters.AddWithValue("@LockTimeout", WorkerConstants.LockTimeoutMinutes);

            command.Parameters.AddWithValue("@WorkerId", WorkerConstants.WorkerId);

            await using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    messages.Add(new OutboxMessage
                    {
                        Id = reader.GetGuid(reader.GetOrdinal("Id")),

                        EventType = reader.GetString(reader.GetOrdinal("EventType")),

                        Payload = reader.GetString(reader.GetOrdinal("Payload")),

                        Status = reader.GetInt32(reader.GetOrdinal("Status")),

                        RetryCount = reader.GetInt32(reader.GetOrdinal("RetryCount")),

                        CreatedAt = reader.GetDateTime(reader.GetOrdinal("CreatedAt")),

                        ProcessedAt =
                            reader.IsDBNull(reader.GetOrdinal("ProcessedAt"))
                            ? null
                            : reader.GetDateTime(reader.GetOrdinal("ProcessedAt")),

                        LockedBy =
                            reader.IsDBNull(reader.GetOrdinal("LockedBy"))
                            ? null
                            : reader.GetString(reader.GetOrdinal("LockedBy")),

                        LockedAt =
                            reader.IsDBNull(reader.GetOrdinal("LockedAt"))
                            ? DateTime.Now
                            : reader.GetDateTime(reader.GetOrdinal("LockedAt"))
                    });
                }
            }

            await transaction.CommitAsync();

            return messages;
        }

        public async Task BulkMarkProcessedAsync(List<Guid> ids)
        {
            if (ids.Count == 0) return;

            await using var db = await _dbFactory.CreateDbContextAsync();

            await using var connection = (SqlConnection) db.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open) await connection.OpenAsync();

            var table = new DataTable();

            table.Columns.Add("Id", typeof(Guid));

            foreach (var id in ids)
            {
                table.Rows.Add(id);
            }

            await using var command = connection.CreateCommand();

            command.CommandText =
            """
            UPDATE o
            SET
                Status = @Processed,
                ProcessedAt = SYSUTCDATETIME(),
                LockedBy = NULL,
                LockedAt = NULL
            FROM OutboxMessages o
            INNER JOIN @Ids ids
                ON o.Id = ids.Id;
            """;

            command.Parameters.AddWithValue("@Processed", DbConstants.Processed);

            var idsParam = command.Parameters.AddWithValue("@Ids", table);

            idsParam.SqlDbType = SqlDbType.Structured;

            idsParam.TypeName = "GuidListType";

            await command.ExecuteNonQueryAsync();
        }

        // Retry Logic
        public async Task BulkIncrementRetryAsync(List<Guid> ids)
        {
            if (ids.Count == 0) return;

            await using var db = await _dbFactory.CreateDbContextAsync();

            await using var connection = (SqlConnection) db.Database.GetDbConnection();

            if (connection.State != ConnectionState.Open) await connection.OpenAsync();

            var table = new DataTable();

            table.Columns.Add("Id", typeof(Guid));

            foreach (var id in ids)
            {
                table.Rows.Add(id);
            }

            await using var command = connection.CreateCommand();

            command.CommandText =
            """
            UPDATE o
            SET
                RetryCount = RetryCount + 1,
                LockedBy = NULL,
                LockedAt = NULL
            FROM OutboxMessages o
            INNER JOIN @Ids ids
                ON o.Id = ids.Id;
            """;

            var idsParam = command.Parameters.AddWithValue("@Ids", table);

            idsParam.SqlDbType = SqlDbType.Structured;

            idsParam.TypeName = "GuidListType";

            await command.ExecuteNonQueryAsync();
        }
    }

}
