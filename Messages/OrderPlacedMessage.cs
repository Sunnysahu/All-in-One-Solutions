namespace Messages
{
    public record OrderPlacedMessage(Guid OrderId, string CustomerName, decimal Amount);
}
