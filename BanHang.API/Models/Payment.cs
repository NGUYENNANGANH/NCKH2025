using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHang.API.Models
{
    public enum PaymentStatus
    {
        Pending = 0,
        Success = 1,
        Failed = 2,
        Refunded = 3,
        Cancelled = 4
    }

    public class Payment
    {
        [Key]
        public string Id_Payment { get; set; } = Guid.NewGuid().ToString();
        
        [ForeignKey("DonHang")]
        public string Id_DonHang { get; set; } = string.Empty;
        
        [ForeignKey("User")]
        public string Id_User { get; set; } = string.Empty;
        
        public decimal Amount { get; set; }
        
        [MaxLength(20)]
        public string PaymentMethod { get; set; } = string.Empty; // VNPAY, MOMO, COD, ...
        
        public PaymentStatus PaymentStatus { get; set; } = PaymentStatus.Pending;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        public DateTime? PaymentDate { get; set; }
        
        [MaxLength(255)]
        public string OrderInfo { get; set; } = string.Empty;
        
        [MaxLength(255)]
        public string TransactionId { get; set; } = string.Empty;
        
        [MaxLength(50)]
        public string? ResponseCode { get; set; }
        
        [MaxLength(255)]
        public string? ResponseMessage { get; set; }
        
        // Navigation Properties
        public virtual DonHang? DonHang { get; set; }
        public virtual ApplicationUser? User { get; set; }
    }
} 