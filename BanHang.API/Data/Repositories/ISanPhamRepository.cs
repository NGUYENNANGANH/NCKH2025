using BanHang.API.Models;

namespace BanHang.API.Data.Repositories
{
    public interface ISanPhamRepository : IRepository<SanPham>
    {
        Task<IEnumerable<SanPham>> GetSanPhamsByDanhMucAsync(int danhMucId);
        Task<IEnumerable<SanPham>> GetSanPhamWithKhuyenMaiAsync();
        Task<SanPham?> GetSanPhamWithDetailsAsync(int id);
        Task<IEnumerable<SanPham>> SearchSanPhamAsync(string keyword);
        Task<bool> IsSKUExistAsync(string sku);
        Task<IEnumerable<SanPham>> GetBestSellingProductsAsync(int limit = 10);
        Task<decimal> GetGiaKhuyenMaiAsync(int sanPhamId);
    }
} 