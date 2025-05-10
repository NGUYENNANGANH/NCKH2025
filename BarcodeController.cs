using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using BanHang.API.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Security.Claims;
using System.IO;
using Microsoft.AspNetCore.Hosting;

namespace BanHang.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BarcodeController : ControllerBase
    {
        private readonly BarcodeService _barcodeService;
        private readonly ILogger<BarcodeController> _logger;
        private readonly IWebHostEnvironment _hostingEnvironment;

        public BarcodeController(
            BarcodeService barcodeService,
            ILogger<BarcodeController> logger,
            IWebHostEnvironment hostingEnvironment)
        {
            _barcodeService = barcodeService;
            _logger = logger;
            _hostingEnvironment = hostingEnvironment;
        }

        /// <summary>
        /// Kiểm tra mã barcode trong database
        /// </summary>
        [HttpGet("check/{barcode}")]
        [Authorize]
        public async Task<IActionResult> CheckBarcode(string barcode)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(barcode))
                {
                    return BadRequest("Mã barcode không được để trống");
                }

                // Lấy User Id nếu đã đăng nhập
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                // Kiểm tra trong database
                var product = await _barcodeService.CheckBarcodeInDatabaseAsync(barcode, userId);

                if (product != null)
                {
                    return Ok(new
                    {
                        status = "success",
                        message = "Tìm thấy sản phẩm trong hệ thống",
                        found = true,
                        product
                    });
                }

                // Không tìm thấy trong database, kiểm tra từ API bên ngoài
                var apiResult = await _barcodeService.LookupBarcodeFromApiAsync(barcode, userId);

                if (apiResult != null && apiResult.Products?.Count > 0)
                {
                    return Ok(new
                    {
                        status = "success",
                        message = "Tìm thấy thông tin sản phẩm từ API bên ngoài",
                        found = true,
                        inDatabase = false,
                        product = apiResult.Products[0]
                    });
                }

                return NotFound(new
                {
                    status = "not_found",
                    message = "Không tìm thấy thông tin cho mã barcode này",
                    found = false
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi kiểm tra barcode: {ex.Message}");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Đã xảy ra lỗi khi xử lý yêu cầu"
                });
            }
        }

        /// <summary>
        /// Scan barcode từ hình ảnh (sử dụng thư viện ZXing hoặc Google Vision API)
        /// </summary>
        [HttpPost("scan")]
        [Authorize]
        public async Task<IActionResult> ScanBarcodeFromImage(IFormFile image)
        {
            try
            {
                if (image == null || image.Length == 0)
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Không có hình ảnh được tải lên"
                    });
                }

                // Lưu file tạm thời để xử lý
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(image.FileName);
                string filePath = Path.Combine(_hostingEnvironment.ContentRootPath, "Temp", fileName);

                // Đảm bảo thư mục Temp tồn tại
                Directory.CreateDirectory(Path.Combine(_hostingEnvironment.ContentRootPath, "Temp"));

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await image.CopyToAsync(stream);
                }

                // TODO: Tích hợp thư viện nhận diện barcode như ZXing hoặc Google Vision API
                // Đây là phần mô phỏng, bạn cần thay thế bằng code thực tế để đọc barcode từ ảnh
                
                // Giả định đã nhận diện được barcode từ ảnh
                string detectedBarcode = "123456789012"; // Thay thế bằng mã thực tế đọc được
                
                // Xóa file tạm sau khi xử lý
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }

                // Chuyển hướng đến endpoint kiểm tra barcode
                return await CheckBarcode(detectedBarcode);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi scan barcode từ hình ảnh: {ex.Message}");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Đã xảy ra lỗi khi xử lý hình ảnh"
                });
            }
        }

        /// <summary>
        /// Lấy lịch sử quét barcode
        /// </summary>
        [HttpGet("history")]
        [Authorize]
        public async Task<IActionResult> GetScanHistory([FromQuery] int limit = 50)
        {
            try
            {
                string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var history = await _barcodeService.GetScanHistoryAsync(userId, limit);

                return Ok(new
                {
                    status = "success",
                    count = history.Count,
                    history
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy lịch sử quét barcode: {ex.Message}");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Đã xảy ra lỗi khi lấy lịch sử quét"
                });
            }
        }

        /// <summary>
        /// Thêm sản phẩm từ thông tin barcode API vào database
        /// </summary>
        [HttpPost("add-product")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddProductFromBarcode([FromBody] AddProductFromBarcodeRequest request)
        {
            try
            {
                if (request == null || string.IsNullOrEmpty(request.Barcode))
                {
                    return BadRequest(new
                    {
                        status = "error",
                        message = "Thông tin sản phẩm không hợp lệ"
                    });
                }

                // Kiểm tra sản phẩm đã tồn tại chưa
                var existingProduct = await _barcodeService.CheckBarcodeInDatabaseAsync(request.Barcode);
                if (existingProduct != null)
                {
                    return Conflict(new
                    {
                        status = "error",
                        message = "Mã barcode đã tồn tại trong hệ thống",
                        product = existingProduct
                    });
                }

                // Tạo sản phẩm mới
                // TODO: Implement thêm sản phẩm vào database
                // Giả sử đã thêm thành công và trả về ID
                string productId = Guid.NewGuid().ToString();

                // Cập nhật lịch sử quét
                if (request.ScanId.HasValue && request.ScanId.Value > 0)
                {
                    await _barcodeService.UpdateBarcodeScanAddedAsync(request.ScanId.Value, productId);
                }

                return Ok(new
                {
                    status = "success",
                    message = "Đã thêm sản phẩm vào hệ thống",
                    productId
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi thêm sản phẩm từ barcode: {ex.Message}");
                return StatusCode(500, new
                {
                    status = "error",
                    message = "Đã xảy ra lỗi khi thêm sản phẩm"
                });
            }
        }
    }

    public class AddProductFromBarcodeRequest
    {
        public string Barcode { get; set; }
        public string TenSanPham { get; set; }
        public double GiaBan { get; set; }
        public string MoTa { get; set; }
        public string HinhAnh { get; set; }
        public int SoLuongTon { get; set; } = 0;
        public string Id_DanhMuc { get; set; }
        public int? ScanId { get; set; }
    }
} 