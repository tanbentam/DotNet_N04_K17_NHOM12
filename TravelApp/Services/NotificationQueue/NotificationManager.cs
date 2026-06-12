using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace TravelApp.Services.NotificationQueue
{
    public partial class NotificationManager : ObservableObject
    {
        private readonly Queue<NotificationMessage> _queue =
            new Queue<NotificationMessage>();
        private readonly object _syncRoot = new object();
        private bool _isDisplaying;
        private CancellationTokenSource _displayCancellation;

        [ObservableProperty] private NotificationMessage _currentNotification;
        [ObservableProperty] private bool _isOpen;

        public void ShowNotification(
            string title,
            string message,
            bool isError = false)
        {
            var notification = new NotificationMessage
            {
                Title = string.IsNullOrWhiteSpace(title)
                    ? "Thông báo"
                    : title.Trim(),
                Message = message?.Trim() ?? string.Empty,
                IsError = isError
            };

            lock (_syncRoot)
            {
                _queue.Enqueue(notification);
            }

            RunOnUiThread(() => _ = ProcessQueueAsync());
        }

        [RelayCommand]
        private void Dismiss()
        {
            _displayCancellation?.Cancel();
        }

        private async Task ProcessQueueAsync()
        {
            if (_isDisplaying)
            {
                return;
            }

            _isDisplaying = true;
            try
            {
                while (TryDequeue(out var notification))
                {
                    CurrentNotification = notification;
                    IsOpen = true;
                    _displayCancellation = new CancellationTokenSource();

                    try
                    {
                        await Task.Delay(
                            notification.IsError ? 5000 : 3500,
                            _displayCancellation.Token);
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    finally
                    {
                        _displayCancellation.Dispose();
                        _displayCancellation = null;
                    }

                    IsOpen = false;
                    await Task.Delay(250);
                }
            }
            finally
            {
                _isDisplaying = false;
            }
        }

        private bool TryDequeue(out NotificationMessage notification)
        {
            lock (_syncRoot)
            {
                if (_queue.Count == 0)
                {
                    notification = null;
                    return false;
                }

                notification = _queue.Dequeue();
                return true;
            }
        }

        private static void RunOnUiThread(Action action)
        {
            var dispatcher = Application.Current?.Dispatcher;
            if (dispatcher == null || dispatcher.CheckAccess())
            {
                action();
                return;
            }

            dispatcher.BeginInvoke(action);
        }
    }
}
