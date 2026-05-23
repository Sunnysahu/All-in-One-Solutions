using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Web_Hook.Data;
using Web_Hook.DTOs;
using Web_Hook.Models;
using Web_Hook.Services.Interfaces;

namespace Web_Hook.Services
{
    public class WebhookService : IWebhookService
    {
        private readonly ApplicationDbContext _dbContext;
        private readonly ILogger<IWebhookService> _logger;

        public WebhookService(ApplicationDbContext dbContext, ILogger<IWebhookService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<bool> IsDuplicateAsync(string eventId)
        {
            return await _dbContext.ProcessedWebhooks.AnyAsync(x => x.EventId == eventId);
        }

        public async Task SaveWebhookEventAsync(WebhookEvent webhookEvent)
        {
            _dbContext.WebhookEvents.Add(webhookEvent);

            await _dbContext.SaveChangesAsync();
        }
        public async Task ProcessWebhookAsync(WebhookEvent webhookEvent)
        {
            try
            {
                // Only Uncomment this code if needed to see the Increasing pool connections
                // await using var transaction = await _dbContext.Database.BeginTransactionAsync();

                var dto = JsonSerializer.Deserialize <RazorpayWebhookDto> (webhookEvent.Payload);

                var paymentEntity = dto!.Payload.Payment.Entity;

                var payment = new Payment
                {
                    OrderId = paymentEntity.OrderId,
                    PaymentId = paymentEntity.Id,
                    Amount = paymentEntity.Amount,
                    Currency = paymentEntity.Currency,
                    Status = paymentEntity.Status,
                    CreatedAt = DateTime.Now
                };
                
                await Task.Delay(2000);
                _dbContext.payment.Add(payment);
                _dbContext.ProcessedWebhooks.Add(new ProcessedWebhook
                {
                    EventId = webhookEvent.EventId,
                    ProcessedAt = DateTime.Now
                });

                //await Task.Delay(2000);
                _dbContext.WebhookEvents.Attach(webhookEvent);

                webhookEvent.IsProcessed = true;
                webhookEvent.ProcessedAt = DateTime.Now;

                // Only Uncomment this code if needed to see the Increasing pool connections

                //await using var transactions = await _dbContext.Database.BeginTransactionAsync();

                //await _dbContext.Database.ExecuteSqlRawAsync(
                //    "WAITFOR DELAY '00:00:30'");

                //await Task.Delay(2000);
                await _dbContext.SaveChangesAsync();

                _logger.LogInformation("Webhook processed successfully. EventId: {EventId}",
                webhookEvent.EventId);
            }
            catch (DbUpdateException ex)
            {
                // SQL UNIQUE constraint violation
                if (ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601))
                {
                    _logger.LogWarning("Duplicate webhook ignored: {EventId}", webhookEvent.EventId);

                    return;
                }

                throw;
            }
            catch (Exception ex)
            {
                webhookEvent.RetryCount++;

                await _dbContext.SaveChangesAsync();

                _logger.LogError(ex,"Webhook processing failed. EventId: {EventId}",webhookEvent.EventId);

                throw;
            }
            finally
            {
                _logger.LogInformation("Completed");
            }
        }

    }
}
