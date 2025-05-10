using BanHang.API.Models;
using System.ComponentModel.DataAnnotations;

namespace BanHang.API.DTOs
{
    public class CreatePaymentDto
    {
        [Required]
        public string OrderId { get; set; } = string.Empty;
        
        [MaxLength(20)]
        public string? BankCode { get; set; }
    }
    
    public class PaymentResponseDto
    {
        public string PaymentUrl { get; set; } = string.Empty;
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Message { get; set; } = string.Empty;
    }
    
    public class PaymentResultDto
    {
        public string OrderId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public PaymentStatus PaymentStatus { get; set; }
        public string ResponseCode { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public bool IsSuccess { get; set; }
    }
    
    public class PaymentInfoDto
    {
        public string Id_Payment { get; set; } = string.Empty;
        public string Id_DonHang { get; set; } = string.Empty;
        public string Id_User { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public PaymentStatus PaymentStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? PaymentDate { get; set; }
        public string OrderInfo { get; set; } = string.Empty;
        public string TransactionId { get; set; } = string.Empty;
        public string? ResponseCode { get; set; }
        public string? ResponseMessage { get; set; }
    }
} 