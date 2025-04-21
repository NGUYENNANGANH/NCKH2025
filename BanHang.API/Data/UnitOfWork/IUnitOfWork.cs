using BanHang.API.Data.Repositories;

namespace BanHang.API.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ISanPhamRepository SanPhamRepository { get; }
        // TODO: Thêm các repository khác khi cần
        
        Task<int> SaveAsync();
        Task BeginTransactionAsync();
        Task CommitAsync();
        Task RollbackAsync();
    }
} 