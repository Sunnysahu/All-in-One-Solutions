using Outbox_Pattern.Common;

namespace Outbox_Pattern.Errors
{
    public static class OrderErrors
    {
        public static readonly Response ServiceUnavailabe =
            new(
                529,
                "SERVICE UNAVAILABE",
                "Service Unavailable",
                "The requested service does not exist."
            );

        public static readonly Response NotFound =
            new(
                404,
                "NOT_FOUND",
                "Resource Not Found",
                "The requested resource does not exist."
            );
    }
}
