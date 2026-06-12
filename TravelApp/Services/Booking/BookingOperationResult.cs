namespace TravelApp.Services.Booking
{
    public sealed class BookingOperationResult
    {
        private BookingOperationResult(bool succeeded, string message)
        {
            Succeeded = succeeded;
            Message = message;
        }

        public bool Succeeded { get; }
        public string Message { get; }

        public static BookingOperationResult Success(string message)
        {
            return new BookingOperationResult(true, message);
        }

        public static BookingOperationResult Failure(string message)
        {
            return new BookingOperationResult(false, message);
        }
    }
}
