namespace Outbox_Pattern.Common
{
    public record Response(int StatusCode, string Code, string Message, string Description);
}
