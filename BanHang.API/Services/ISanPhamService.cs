using BanHang.API.DTOs;

namespace BanHang.API.Services
{
    public interface ISanPhamService
    {
        Task<decimal?> TinhGiaKhuyenMai(int idSanPham, decimal giaBan);
        Task<List<KhuyenMaiApDungDto>> GetKhuyenMaiApDung(int idSanPham);
    }
} 