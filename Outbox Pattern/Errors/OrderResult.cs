using Outbox_Pattern.Common;

namespace Outbox_Pattern.Errors
{
    public static class OrderResult
    {

        public static readonly Response Success =
            new(
                200,
                "ORDER_SUCCESSFUL",
                "Order Placed Successful",
                "Order Placed is Successful"
            );
    }
}
