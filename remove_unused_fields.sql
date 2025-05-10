USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Đang xóa các trường không sử dụng...';
    
    -- 1. Xóa cột Id_NguoiTao và Id_NguoiCapNhat khỏi bảng SanPham_KhuyenMais
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SanPham_KhuyenMais') AND name = 'Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP COLUMN [Id_NguoiTao];
        PRINT 'Đã xóa cột Id_NguoiTao khỏi bảng SanPham_KhuyenMais';
    END
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SanPham_KhuyenMais') AND name = 'Id_NguoiCapNhat')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP COLUMN [Id_NguoiCapNhat];
        PRINT 'Đã xóa cột Id_NguoiCapNhat khỏi bảng SanPham_KhuyenMais';
    END
    
    -- 2. Xóa cột Id_NguoiTao khỏi bảng Vouchers
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Vouchers') AND name = 'Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP COLUMN [Id_NguoiTao];
        PRINT 'Đã xóa cột Id_NguoiTao khỏi bảng Vouchers';
    END
    
    COMMIT TRANSACTION;
    PRINT 'Đã xóa tất cả các trường không sử dụng!';
    PRINT 'Lưu ý: Hãy làm mới (refresh) SQL Server Management Studio và tạo lại diagram để thấy các thay đổi';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 