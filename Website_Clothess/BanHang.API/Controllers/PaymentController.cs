using BanHang.API.Data;
using BanHang.API.DTOs;
using BanHang.API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Net;
using System.Security.Claims;
using System.Text;

namespace BanHang.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PaymentController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentController> _logger;

        public PaymentController(ApplicationDbContext context, IConfiguration configuration, ILogger<PaymentController> logger)
        {
            _context = context;
            _configuration = configuration;
            _logger = logger;
        }

        // POST: api/Payment/create-payment
        [HttpPost("create-payment")]
        [Authorize]
        public async Task<ActionResult<PaymentResponseDto>> CreatePayment([FromBody] CreatePaymentDto paymentDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized();
            }

            // Tìm đơn hàng
            var donHang = await _context.DonHangs.FindAsync(paymentDto.OrderId);
            if (donHang == null)
            {
                return NotFound("Không tìm thấy đơn hàng");
            }

            if (donHang.Id_KH != userId)
            {
                return Forbid("Bạn không có quyền truy cập đơn hàng này");
            }

            if (donHang.TrangThai != TrangThaiDonHang.ChoXacNhan)
            {
                return BadRequest("Đơn hàng không ở trạng thái chờ xác nhận");
            }

            try
            {
                // Lấy thông tin cấu hình từ appsettings.json
                string vnp_Url = _configuration["VnPay:Url"] ?? "https://sandbox.vnpayment.vn/paymentv2/vpcpay.html";
                string vnp_TmnCode = _configuration["VnPay:TmnCode"] ?? "DEMO";
                string vnp_HashSecret = _configuration["VnPay:HashSecret"] ?? "ABCDEFGHIJKLMNOPQRSTUVWXYZABCDEF";
                string vnp_ReturnUrl = _configuration["VnPay:ReturnUrl"] ?? "http://localhost:3000/payment/result";

                // Tạo các tham số cho VNPAY
                VnPayLibrary vnpay = new VnPayLibrary();
                vnpay.AddRequestData("vnp_Version", "2.1.0");
                vnpay.AddRequestData("vnp_Command", "pay");
                vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode);
                vnpay.AddRequestData("vnp_Amount", (donHang.TongTien * 100).ToString()); // Nhân 100 để chuyển sang số tiền VNĐ
                vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
                vnpay.AddRequestData("vnp_CurrCode", "VND");
                vnpay.AddRequestData("vnp_IpAddr", GetIpAddress());
                vnpay.AddRequestData("vnp_Locale", "vn");
                vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {donHang.Id_DonHang}");
                vnpay.AddRequestData("vnp_OrderType", "270000"); // Mã danh mục hàng hóa
                vnpay.AddRequestData("vnp_ReturnUrl", vnp_ReturnUrl);
                vnpay.AddRequestData("vnp_TxnRef", donHang.Id_DonHang); // Mã tham chiếu giao dịch

                // Thêm thông tin bill
                vnpay.AddRequestData("vnp_BankCode", string.IsNullOrEmpty(paymentDto.BankCode) ? "" : paymentDto.BankCode);
                
                string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, vnp_HashSecret);

                // Lưu thông tin thanh toán
                var payment = new Payment
                {
                    Id_DonHang = donHang.Id_DonHang,
                    Id_User = userId,
                    Amount = donHang.TongTien,
                    PaymentMethod = "VNPAY",
                    PaymentStatus = PaymentStatus.Pending,
                    CreatedAt = DateTime.Now,
                    OrderInfo = $"Thanh toan don hang {donHang.Id_DonHang}",
                    TransactionId = donHang.Id_DonHang
                };

                _context.Payments.Add(payment);
                await _context.SaveChangesAsync();

                return Ok(new PaymentResponseDto
                {
                    PaymentUrl = paymentUrl,
                    OrderId = donHang.Id_DonHang,
                    Amount = donHang.TongTien,
                    Message = "Url thanh toán được tạo thành công"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi tạo URL thanh toán VNPAY");
                return StatusCode(500, "Đã xảy ra lỗi khi tạo URL thanh toán");
            }
        }

        // GET: api/Payment/payment-result
        [HttpGet("payment-result")]
        public async Task<ActionResult<PaymentResultDto>> PaymentResult([FromQuery] string vnp_ResponseCode, [FromQuery] string vnp_TxnRef)
        {
            try
            {
                // Kiểm tra mã phản hồi từ VNPAY
                bool isSuccess = vnp_ResponseCode == "00";
                var message = GetResponseMessage(vnp_ResponseCode);

                // Tìm thông tin đơn hàng
                var donHang = await _context.DonHangs.FindAsync(vnp_TxnRef);
                if (donHang == null)
                {
                    return NotFound("Không tìm thấy đơn hàng");
                }

                // Tìm thông tin thanh toán
                var payment = await _context.Payments
                    .Where(p => p.Id_DonHang == vnp_TxnRef && p.PaymentMethod == "VNPAY")
                    .OrderByDescending(p => p.CreatedAt)
                    .FirstOrDefaultAsync();

                if (payment == null)
                {
                    return NotFound("Không tìm thấy thông tin thanh toán");
                }

                // Cập nhật trạng thái thanh toán
                payment.PaymentStatus = isSuccess ? PaymentStatus.Success : PaymentStatus.Failed;
                payment.ResponseCode = vnp_ResponseCode;
                payment.ResponseMessage = message;
                payment.PaymentDate = DateTime.Now;

                // Nếu thanh toán thành công, cập nhật trạng thái đơn hàng
                if (isSuccess)
                {
                    donHang.TrangThai = TrangThaiDonHang.DangXuLy;
                    _context.Entry(donHang).State = EntityState.Modified;
                }

                _context.Entry(payment).State = EntityState.Modified;
                await _context.SaveChangesAsync();

                return Ok(new PaymentResultDto
                {
                    OrderId = donHang.Id_DonHang,
                    Amount = donHang.TongTien,
                    PaymentStatus = payment.PaymentStatus,
                    ResponseCode = vnp_ResponseCode,
                    Message = message,
                    IsSuccess = isSuccess
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý kết quả thanh toán VNPAY");
                return StatusCode(500, "Đã xảy ra lỗi khi xử lý kết quả thanh toán");
            }
        }

        private string GetIpAddress()
        {
            string ipAddress;
            try
            {
                ipAddress = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "127.0.0.1";
                
                if (string.IsNullOrEmpty(ipAddress) || ipAddress == "::1")
                    ipAddress = "127.0.0.1";
            }
            catch (Exception)
            {
                ipAddress = "127.0.0.1";
            }

            return ipAddress;
        }

        private string GetResponseMessage(string responseCode)
        {
            switch (responseCode)
            {
                case "00":
                    return "Giao dịch thành công";
                case "07":
                    return "Trừ tiền thành công. Giao dịch bị nghi ngờ (liên quan tới lừa đảo, gian lận)";
                case "09":
                    return "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng chưa đăng ký dịch vụ InternetBanking";
                case "10":
                    return "Giao dịch không thành công do: Khách hàng xác thực thông tin thẻ/tài khoản không đúng quá 3 lần";
                case "11":
                    return "Giao dịch không thành công do: Đã hết hạn chờ thanh toán";
                case "12":
                    return "Giao dịch không thành công do: Thẻ/Tài khoản của khách hàng bị khóa";
                case "13":
                    return "Giao dịch không thành công do Quý khách nhập sai mật khẩu xác thực giao dịch (OTP)";
                case "24":
                    return "Giao dịch không thành công do: Khách hàng hủy giao dịch";
                case "51":
                    return "Giao dịch không thành công do: Tài khoản của quý khách không đủ số dư để thực hiện giao dịch";
                case "65":
                    return "Giao dịch không thành công do: Tài khoản của Quý khách đã vượt quá hạn mức giao dịch trong ngày";
                case "75":
                    return "Ngân hàng thanh toán đang bảo trì";
                case "79":
                    return "Giao dịch không thành công do: KH nhập sai mật khẩu thanh toán quá số lần quy định";
                case "99":
                    return "Lỗi không xác định";
                default:
                    return "Lỗi giao dịch";
            }
        }
    }

    // Class tiện ích cho VNPAY
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayComparer());
        
        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }

        public string CreateRequestUrl(string baseUrl, string vnp_HashSecret)
        {
            StringBuilder data = new StringBuilder();
            
            foreach (KeyValuePair<string, string> kv in _requestData)
            {
                if (!string.IsNullOrEmpty(kv.Value))
                {
                    data.Append(WebUtility.UrlEncode(kv.Key) + "=" + WebUtility.UrlEncode(kv.Value) + "&");
                }
            }
            
            string queryString = data.ToString();
            baseUrl += "?" + queryString;
            string signData = queryString;
            if (signData.Length > 0)
            {
                signData = signData.Remove(signData.Length - 1, 1);
            }
            
            string vnp_SecureHash = HmacSHA512(vnp_HashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnp_SecureHash;

            return baseUrl;
        }

        private string HmacSHA512(string key, string inputData)
        {
            var hmac = new System.Security.Cryptography.HMACSHA512(Encoding.UTF8.GetBytes(key));
            var hashData = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            var sb = new StringBuilder();
            
            foreach (var b in hashData)
            {
                sb.Append(b.ToString("x2"));
            }

            return sb.ToString();
        }

        public bool ValidateSignature(string inputHash, string secretKey, string requestRaw)
        {
            string myChecksum = HmacSHA512(secretKey, requestRaw);
            return myChecksum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }
    }

    public class VnPayComparer : IComparer<string>
    {
        public int Compare(string x, string y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpCompare = CompareInfo.GetCompareInfo("en-US");
            return vnpCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
} 