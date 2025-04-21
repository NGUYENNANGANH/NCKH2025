/*
 * Hướng dẫn cấu hình Startup.cs cho tính năng Barcode
 * Thêm đoạn code sau vào file Startup.cs trong dự án của bạn
 */

// 1. Thêm các using statements sau vào đầu file
using BanHang.API.Services;
using System;

// 2. Trong phương thức ConfigureServices, thêm đoạn code sau:

// Đăng ký HTTP Client cho Barcode API
services.AddHttpClient("BarcodeAPI", client =>
{
    client.BaseAddress = new Uri(Configuration["ExternalApis:BarcodeApi:BaseUrl"]);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

// Đăng ký BarcodeService
services.AddScoped<BarcodeService>();

// 3. Cập nhật appsettings.json, thêm cấu hình sau:
/*
"ExternalApis": {
  "BarcodeApi": {
    "BaseUrl": "https://api.barcodelookup.com/v3/",
    "ApiKey": "YOUR_API_KEY"
  }
}
*/

// 4. Nếu muốn sử dụng Google Vision API để nhận dạng barcode từ ảnh
// Thêm package: Google.Cloud.Vision.V1
// Và thêm cấu hình:
/*
services.AddSingleton(provider =>
{
    // Đường dẫn đến file JSON key của Google Cloud
    string keyFilePath = Path.Combine(
        provider.GetRequiredService<IWebHostEnvironment>().ContentRootPath,
        "google-vision-key.json");
    
    // Khởi tạo client
    var builder = new ImageAnnotatorClientBuilder
    {
        CredentialsPath = keyFilePath
    };
    
    return builder.Build();
});
*/

// 5. Tạo thư mục Temp trong thư mục gốc của dự án để lưu file tạm khi xử lý ảnh
/*
var tempDirectory = Path.Combine(env.ContentRootPath, "Temp");
if (!Directory.Exists(tempDirectory))
{
    Directory.CreateDirectory(tempDirectory);
}
*/

// 6. Cài đặt các NuGet packages cần thiết:
// Microsoft.Extensions.Http
// Newtonsoft.Json
// System.Data.SqlClient
// ZXing.Net (nếu sử dụng ZXing để đọc barcode từ ảnh)
// Google.Cloud.Vision.V1 (nếu sử dụng Google Vision API)

// 7. Đảm bảo đã thêm Authentication và Authorization cho API
/*
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        // Cấu hình JWT
    });

services.AddAuthorization(options =>
{
    // Cấu hình policy nếu cần
});
*/ 