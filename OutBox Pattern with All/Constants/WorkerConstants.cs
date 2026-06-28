namespace OutBox_Pattern_with_All.Constants
{
    public static class WorkerConstants
    {
        public static readonly string WorkerId = Environment.MachineName + "-" + Guid.NewGuid();

        public const int BatchSize = 100;

        public const int LockTimeoutMinutes = 5;
    }
}
