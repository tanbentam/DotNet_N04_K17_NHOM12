using System;
using System.IO;
using System.Threading.Tasks;
using TravelApp.Frontend.Services.Logging;

namespace TravelApp.Frontend.Services.ImageManagement
{
    public class ImageUploadService
    {
        public async Task<string> UploadImageAsync(string localFilePath, string targetType)
        {
            // API INTEGRATION POINT:
            // Replace the mock delay with POST /api/uploads using multipart/form-data or base64.
            // Expected response: { imageUrl } for Destination or Hotel image records.
            if (!File.Exists(localFilePath))
            {
                throw new FileNotFoundException("Image file was not found.", localFilePath);
            }

            try
            {
                byte[] imageBytes = await Task.Run(() => File.ReadAllBytes(localFilePath));
                string base64Image = Convert.ToBase64String(imageBytes);

                await Task.Delay(500);

                return $"https://backend-storage.com/images/{targetType.ToLower()}/{Guid.NewGuid()}.jpg";
            }
            catch (Exception ex)
            {
                LoggerService.LogApiError("UploadImageAsync", ex.Message);
                return null;
            }
        }
    }
}
