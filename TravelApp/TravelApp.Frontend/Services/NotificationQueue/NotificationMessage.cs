namespace TravelApp.Frontend.Services.NotificationQueue
{
    public class NotificationMessage
    {
        public string Title { get; set; }
        public string Message { get; set; }
        public bool IsError { get; set; }
    }
}