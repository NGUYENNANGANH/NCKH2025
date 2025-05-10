using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHang.API.Models
{
    public class HoaDon
    {
        [Key]
        public int Id_HoaDon { get; set; }
        
        public DateTime CreateAt { get; set; } = DateTime.Now;
        
        [ForeignKey("User")]
        public string Id_User { get; set; } = string.Empty;
        
        [ForeignKey("DonHang")]
        public string Id_DonHang { get; set; } = string.Empty;
        
        public DateTime? DateXuat { get; set; }
        
        [MaxLength(50)]
        public string? SoHoaDon { get; set; }
        
        // Navigation Properties
        public virtual ApplicationUser? User { get; set; }
        public virtual DonHang? DonHang { get; set; }
    }
} 