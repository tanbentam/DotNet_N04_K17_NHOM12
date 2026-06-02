using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TravelApp.Frontend.Services.NotificationQueue
{
    // Cần đăng ký class này dưới dạng Singleton trong App.xaml.cs
    public partial class NotificationManager : ObservableObject
    {
        private readonly Queue<NotificationMessage> _queue = new Queue<NotificationMessage>();
        private bool _isDisplaying;

        [ObservableProperty]
        private NotificationMessage _currentNotification;

        [ObservableProperty]
        private bool _isOpen;

        [cite_start]// Phương thức để thêm thông báo vào hàng đợi 
        public void ShowNotification(string title, string message, bool isError = false)
        {
            _queue.Enqueue(new NotificationMessage { Title = title, Message = message, IsError = isError });
            ProcessQueueAsync();
        }

        private async void ProcessQueueAsync()
        {
            if (_isDisplaying || _queue.Count == 0)
                return;

            _isDisplaying = true;
            CurrentNotification = _queue.Dequeue();
            IsOpen = true;

            // Hiển thị popup trong 3 giây
            await Task.Delay(3000);

            IsOpen = false;
            _isDisplaying = false;

            // Chờ animation đóng (giả lập 0.5s) trước khi hiện popup tiếp theo
            await Task.Delay(500);
            ProcessQueueAsync();
        }
    }
}