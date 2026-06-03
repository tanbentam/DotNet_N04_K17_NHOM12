using System;
using System.IO;
using System.Threading.Tasks;

namespace TravelApp.Services.ImageManagement
{
    public class ImageUploadService
    {
        // [BACKEND DEVELOPER NOTE] 
        // Endpoint nhận ảnh: Constants.Base_API_Url + "upload"
        // Dữ liệu sẽ được gửi dưới dạng MultipartFormDataContent hoặc Base64.

        public async Task<string> UploadImageAsync(string localFilePath, string targetType)
        {
            // targetType: "Destination" hoặc "Hotel"
            if (!File.Exists(localFilePath))
                throw new FileNotFoundException("Không tìm thấy file ảnh.");

            try
            {
                // Đọc file ảnh dưới dạng byte array
                byte[] imageBytes = await Task.Run(() => File.ReadAllBytes(localFilePath));
                string base64Image = Convert.ToBase64String(imageBytes);

                // [BACKEND DEVELOPER NOTE]
                // Thực hiện HTTP POST call tới Backend với chuỗi base64Image.
                // Backend sẽ lưu trữ và trả về URL của hình ảnh.

                await Task.Delay(500); // Giả lập API call

                // Trả về URL giả lập
                return $"https://backend-storage.com/images/{targetType.ToLower()}/{Guid.NewGuid()}.jpg";
            }
            catch (Exception ex)
            {
                // Ghi log lỗi API
                LoggerService.LogApiError("UploadImageAsync", ex.Message);
                return null;
            }
        }
    }
}