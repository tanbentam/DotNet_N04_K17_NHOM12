using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TravelApp.Services.Logging;

namespace TravelApp.Services.ImageManagement
{
    public sealed class ImageUploadService
    {
        public const long MaximumFileSize = 5 * 1024 * 1024;

        private static readonly HashSet<string> AllowedExtensions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".bmp",
                ".gif"
            };

        public string SelectImageFile()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Chọn ảnh",
                Filter =
                    "Image files (*.jpg;*.jpeg;*.png;*.bmp;*.gif)|" +
                    "*.jpg;*.jpeg;*.png;*.bmp;*.gif",
                CheckFileExists = true,
                Multiselect = false
            };

            return dialog.ShowDialog() == true
                ? dialog.FileName
                : null;
        }

        public async Task<string> UploadImageAsync(
            string localFilePath,
            string targetType)
        {
            ValidateTargetType(targetType);

            try
            {
                return await Task.Run(() =>
                {
                    ValidateImage(localFilePath);
                    return Path.GetFullPath(localFilePath);
                });
            }
            catch (ImageUploadException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LoggerService.LogException(
                    "Prepare image upload",
                    ex,
                    "TargetType=" + targetType);
                throw new ImageUploadException(
                    "Không thể đọc file ảnh đã chọn.",
                    ex);
            }
        }

        private static void ValidateTargetType(string targetType)
        {
            if (!string.Equals(
                    targetType,
                    "Destination",
                    StringComparison.OrdinalIgnoreCase) &&
                !string.Equals(
                    targetType,
                    "Hotel",
                    StringComparison.OrdinalIgnoreCase))
            {
                throw new ImageUploadException(
                    "Loại nội dung tải ảnh không hợp lệ.");
            }
        }

        private static void ValidateImage(string localFilePath)
        {
            if (string.IsNullOrWhiteSpace(localFilePath) ||
                !File.Exists(localFilePath))
            {
                throw new ImageUploadException(
                    "Không tìm thấy file ảnh đã chọn.");
            }

            var extension = Path.GetExtension(localFilePath);
            if (!AllowedExtensions.Contains(extension))
            {
                throw new ImageUploadException(
                    "Chỉ hỗ trợ ảnh JPG, JPEG, PNG, BMP hoặc GIF.");
            }

            var fileInfo = new FileInfo(localFilePath);
            if (fileInfo.Length <= 0)
            {
                throw new ImageUploadException("File ảnh đang trống.");
            }

            if (fileInfo.Length > MaximumFileSize)
            {
                throw new ImageUploadException(
                    "Kích thước ảnh không được vượt quá 5 MB.");
            }

            try
            {
                using (var stream = File.Open(
                    localFilePath,
                    FileMode.Open,
                    FileAccess.Read,
                    FileShare.Read))
                {
                    var decoder = BitmapDecoder.Create(
                        stream,
                        BitmapCreateOptions.PreservePixelFormat,
                        BitmapCacheOption.OnLoad);
                    if (decoder.Frames.Count == 0 ||
                        decoder.Frames[0].PixelWidth <= 0 ||
                        decoder.Frames[0].PixelHeight <= 0)
                    {
                        throw new ImageUploadException(
                            "File đã chọn không chứa ảnh hợp lệ.");
                    }
                }
            }
            catch (ImageUploadException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new ImageUploadException(
                    "File đã chọn không phải ảnh hợp lệ hoặc đã bị hỏng.",
                    ex);
            }
        }
    }

    public sealed class ImageUploadException : Exception
    {
        public ImageUploadException(string message)
            : base(message)
        {
        }

        public ImageUploadException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
