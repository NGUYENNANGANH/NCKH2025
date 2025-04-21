using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BanHang.API.Models
{
    [Table("DanhGia")]
    public class DanhGia
    {
        [Key]
        public string Id_DanhGia { get; set; } = Guid.NewGuid().ToString();
        
        [Required]
        public string Comment { get; set; } = string.Empty;
        
        [Required]
        [Range(1, 5)]
        public int Vote { get; set; }
        
        [Required]
        public string Id_User { get; set; } = string.Empty;
        
        [ForeignKey("Id_User")]
        public ApplicationUser? User { get; set; }
        
        [Required]
        public int Id_SanPham { get; set; }
        
        [ForeignKey("Id_SanPham")]
        public SanPham? SanPham { get; set; }
        
        public DateTime Date { get; set; } = DateTime.Now;
    }
} 