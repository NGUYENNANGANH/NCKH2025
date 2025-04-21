using BanHang.API.Models;
using Microsoft.EntityFrameworkCore;

namespace BanHang.API.Data.Repositories
{
    public class SanPhamRepository : Repository<SanPham>, ISanPhamRepository
    {
        public SanPhamRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<SanPham>> GetSanPhamsByDanhMucAsync(int danhMucId)
        {
            return await _context.SanPhams
                .Where(s => s.Id_DanhMuc == danhMucId && s.TrangThaiSanPham == TrangThaiSanPham.DangBan)
                .Include(s => s.DanhMuc)
                .ToListAsync();
        }

        public async Task<IEnumerable<SanPham>> GetSanPhamWithKhuyenMaiAsync()
        {
            return await _context.SanPhams
                .Include(s => s.DanhMuc)
                .Include(s => s.SanPham_KhuyenMais)
                .ThenInclude(sk => sk.KhuyenMai)
                .Where(s => s.TrangThaiSanPham == TrangThaiSanPham.DangBan && s.SanPham_KhuyenMais.Any(sk => 
                    sk.KhuyenMai != null && 
                    sk.TrangThai &&
                    DateTime.Now >= sk.KhuyenMai.NgayBatDau && 
                    DateTime.Now <= sk.KhuyenMai.NgayKetThuc))
                .ToListAsync();
        }

        public async Task<SanPham?> GetSanPhamWithDetailsAsync(int id)
        {
            return await _context.SanPhams
                .Include(s => s.DanhMuc)
                .Include(s => s.SanPham_KhuyenMais)
                .ThenInclude(sk => sk.KhuyenMai)
                .Include(s => s.DanhGias)
                .ThenInclude(d => d.User)
                .FirstOrDefaultAsync(s => s.Id_SanPham == id);
        }

        public async Task<IEnumerable<SanPham>> SearchSanPhamAsync(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return new List<SanPham>();

            return await _context.SanPhams
                .Where(s => s.TrangThaiSanPham == TrangThaiSanPham.DangBan && 
                    (s.TenSanPham.Contains(keyword) || 
                     s.MoTa != null && s.MoTa.Contains(keyword) ||
                     s.SKU != null && s.SKU.Contains(keyword)))
                .Include(s => s.DanhMuc)
                .ToListAsync();
        }

        public async Task<bool> IsSKUExistAsync(string sku)
        {
            if (string.IsNullOrEmpty(sku))
                return false;

            return await _context.SanPhams
                .AnyAsync(s => s.SKU == sku);
        }

        public async Task<IEnumerable<SanPham>> GetBestSellingProductsAsync(int limit = 10)
        {
            return await _context.SanPhams
                .Where(s => s.TrangThaiSanPham == TrangThaiSanPham.DangBan)
                .OrderByDescending(s => s.SoLuongDaBan)
                .Take(limit)
                .Include(s => s.DanhMuc)
                .ToListAsync();
        }

        public async Task<decimal> GetGiaKhuyenMaiAsync(int sanPhamId)
        {
            var sanPham = await _context.SanPhams
                .Include(s => s.SanPham_KhuyenMais)
                .ThenInclude(sk => sk.KhuyenMai)
                .FirstOrDefaultAsync(s => s.Id_SanPham == sanPhamId);

            if (sanPham == null)
                return 0;

            var now = DateTime.Now;
            
            // Tìm khuyến mãi hiện đang áp dụng và có phần trăm giảm cao nhất
            var khuyenMai = sanPham.SanPham_KhuyenMais?
                .Where(sk => sk.KhuyenMai != null &&
                           sk.TrangThai &&
                           now >= sk.KhuyenMai.NgayBatDau &&
                           now <= sk.KhuyenMai.NgayKetThuc)
                .OrderByDescending(sk => sk.MucGiamGiaRieng ?? sk.KhuyenMai!.PhanTramGiam)
                .FirstOrDefault();

            if (khuyenMai == null)
                return sanPham.GiaBan;

            var phanTramGiam = khuyenMai.MucGiamGiaRieng ?? khuyenMai.KhuyenMai!.PhanTramGiam;
            var discountPercent = phanTramGiam / 100.0m;
            
            return sanPham.GiaBan * (1 - discountPercent);
        }
    }
} 