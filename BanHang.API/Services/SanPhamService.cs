using BanHang.API.Data;
using BanHang.API.DTOs;
using Microsoft.EntityFrameworkCore;

namespace BanHang.API.Services
{
    public class SanPhamService : ISanPhamService
    {
        private readonly ApplicationDbContext _context;

        public SanPhamService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<decimal?> TinhGiaKhuyenMai(int idSanPham, decimal giaBan)
        {
            var khuyenMais = await GetKhuyenMaiHieuLuc(idSanPham);
            
            if (!khuyenMais.Any())
            {
                return null;
            }

            // Tìm khuyến mãi có phần trăm giảm lớn nhất
            var maxKhuyenMai = khuyenMais.OrderByDescending(k => k.PhanTramGiam).First();
            
            // Tính giá sau khi giảm
            decimal tyLeGiam = maxKhuyenMai.PhanTramGiam / 100m;
            decimal giaKhuyenMai = giaBan * (1 - tyLeGiam);
            
            return giaKhuyenMai;
        }

        public async Task<List<KhuyenMaiApDungDto>> GetKhuyenMaiApDung(int idSanPham)
        {
            var khuyenMais = await GetKhuyenMaiHieuLuc(idSanPham);
            
            return khuyenMais.Select(km => new KhuyenMaiApDungDto
            {
                Id_KhuyenMai = km.Id_KhuyenMai,
                TenKhuyenMai = km.TenKhuyenMai ?? "Khuyến mãi",
                TyLeGiam = km.PhanTramGiam,
                NgayBatDau = km.NgayBatDau,
                NgayKetThuc = km.NgayKetThuc
            }).ToList();
        }

        private async Task<List<KhuyenMaiViewModel>> GetKhuyenMaiHieuLuc(int idSanPham)
        {
            var now = DateTime.Now;

            // Lấy các khuyến mãi áp dụng cho toàn hệ thống
            var khuyenMaiToanHeThong = await _context.KhuyenMais
                .Where(km => km.ApDungToanHeThong && 
                           km.NgayBatDau <= now && 
                           km.NgayKetThuc >= now)
                .Select(km => new KhuyenMaiViewModel
                {
                    Id_KhuyenMai = km.Id_KhuyenMai,
                    TenKhuyenMai = km.TenKhuyenMai,
                    PhanTramGiam = km.PhanTramGiam,
                    NgayBatDau = km.NgayBatDau,
                    NgayKetThuc = km.NgayKetThuc
                })
                .ToListAsync();

            // Lấy các khuyến mãi áp dụng riêng cho sản phẩm
            var khuyenMaiRieng = await _context.SanPham_KhuyenMais
                .Where(skm => skm.Id_SanPham == idSanPham && 
                            skm.KhuyenMai != null && 
                            skm.KhuyenMai.NgayBatDau <= now && 
                            skm.KhuyenMai.NgayKetThuc >= now)
                .Select(skm => new KhuyenMaiViewModel
                {
                    Id_KhuyenMai = skm.Id_KhuyenMai,
                    TenKhuyenMai = skm.KhuyenMai.TenKhuyenMai,
                    PhanTramGiam = skm.KhuyenMai.PhanTramGiam,
                    NgayBatDau = skm.KhuyenMai.NgayBatDau,
                    NgayKetThuc = skm.KhuyenMai.NgayKetThuc
                })
                .ToListAsync();

            // Kết hợp hai danh sách
            return khuyenMaiToanHeThong.Concat(khuyenMaiRieng).ToList();
        }

        private class KhuyenMaiViewModel
        {
            public int Id_KhuyenMai { get; set; }
            public string? TenKhuyenMai { get; set; }
            public int PhanTramGiam { get; set; }
            public DateTime NgayBatDau { get; set; }
            public DateTime NgayKetThuc { get; set; }
        }
    }
} 