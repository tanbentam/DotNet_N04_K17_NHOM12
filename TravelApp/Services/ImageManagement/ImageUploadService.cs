using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using TravelApp.Services.Logging;

namespace TravelApp.Services.ImageManagement
{
    public sealed class ImageUploadService
    {
        public const long MaximumFileSize = 5 * 1024 * 1024;
        private readonly string _storageRoot;

        private static readonly HashSet<string> AllowedExtensions =
            new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            {
                ".jpg",
                ".jpeg",
                ".png",
                ".bmp",
                ".gif"
            };

        public ImageUploadService()
        {
            _storageRoot = Path.Combine(
                Environment.GetFolderPath(
                    Environment.SpecialFolder.LocalApplicationData),
                "TravelApp",
                "Images");
        }

        public string StorageRoot => _storageRoot;

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
                    return StoreImage(localFilePath, targetType);
                });
            }
            catch (ImageUploadException)
            {
                throw;
            }
            catch (Exception ex)
            {
                LoggerService.LogException(
                    "Store uploaded image",
                    ex,
                    "TargetType=" + targetType);
                throw new ImageUploadException(
                    "Không thể lưu ảnh vào bộ nhớ cục bộ.",
                    ex);
            }
        }

        private string StoreImage(
            string localFilePath,
            string targetType)
        {
            var targetFolder = Path.Combine(
                _storageRoot,
                NormalizeTargetType(targetType));
            Directory.CreateDirectory(targetFolder);

            var extension = Path.GetExtension(localFilePath).ToLowerInvariant();
            var fileName = ComputeFileHash(localFilePath) + extension;
            var destinationPath = Path.Combine(targetFolder, fileName);

            if (!File.Exists(destinationPath))
            {
                File.Copy(localFilePath, destinationPath, false);
            }

            return destinationPath;
        }

        private static string ComputeFileHash(string localFilePath)
        {
            using (var stream = File.OpenRead(localFilePath))
            using (var sha256 = SHA256.Create())
            {
                return BitConverter.ToString(sha256.ComputeHash(stream))
                    .Replace("-", string.Empty)
                    .ToLowerInvariant();
            }
        }

        private static string NormalizeTargetType(string targetType)
        {
            return string.Equals(
                targetType,
                "Destination",
                StringComparison.OrdinalIgnoreCase)
                ? "Destination"
                : "Hotel";
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
