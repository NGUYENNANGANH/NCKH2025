using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHang.API.Models
{
    public class GioHang
    {
        [Key]
        public string Id_GioHang { get; set; } = Guid.NewGuid().ToString();
        
        [ForeignKey("User")]
        public string Id_User { get; set; } = string.Empty;
        
        public DateTime NgayTao { get; set; } = DateTime.Now;
        
        public DateTime? NgayCapNhat { get; set; }
        
        // Navigation Properties
        public virtual ApplicationUser? User { get; set; }
        public virtual ICollection<ChiTietGioHang>? ChiTietGioHangs { get; set; }
    }
} 