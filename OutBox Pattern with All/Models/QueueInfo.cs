namespace OutBox_Pattern_with_All.Models
{
    public class QueueInfo
    {
        public string Name { get; set; } = string.Empty;

        public int Messages { get; set; }

        public int MessagesReady { get; set; }

        public int MessagesUnacknowledged { get; set; }
    }
}
