namespace TravelApp.Services.NotificationQueue
{
    public class NotificationMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; }

        public string IconKind => IsError
            ? "AlertCircleOutline"
            : "CheckCircleOutline";
    }
}
