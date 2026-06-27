namespace OutBox_Pattern_with_All.Constants
{
    public static class OutboxStatus
    {
        public const int Pending = 0;

        public const int Processing = 1;

        public const int Processed = 2;

        public const int Failed = 3;
    }
}
