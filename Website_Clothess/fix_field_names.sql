USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Đang cập nhật cấu trúc bảng...';
    
    -- 1. Xóa các ràng buộc khóa ngoại liên quan đến Id_NguoiTao và Id_NguoiCapNhat
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP CONSTRAINT [FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiTao];
        PRINT 'Đã xóa FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiTao';
    END
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiCapNhat')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP CONSTRAINT [FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiCapNhat];
        PRINT 'Đã xóa FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiCapNhat';
    END
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Vouchers_TO_AspNetUsers_Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP CONSTRAINT [FK_Vouchers_TO_AspNetUsers_Id_NguoiTao];
        PRINT 'Đã xóa FK_Vouchers_TO_AspNetUsers_Id_NguoiTao';
    END
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Vouchers_AspNetUsers')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP CONSTRAINT [FK_Vouchers_AspNetUsers];
        PRINT 'Đã xóa FK_Vouchers_AspNetUsers';
    END
    
    -- 2. Thay đổi Id_KH trong bảng DonHangs thành User_Id
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DonHangs_TO_AspNetUsers_Id_KH')
    BEGIN
        ALTER TABLE [dbo].[DonHangs] DROP CONSTRAINT [FK_DonHangs_TO_AspNetUsers_Id_KH];
        PRINT 'Đã xóa FK_DonHangs_TO_AspNetUsers_Id_KH';
    END
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DonHangs_AspNetUsers_Id_KH')
    BEGIN
        ALTER TABLE [dbo].[DonHangs] DROP CONSTRAINT [FK_DonHangs_AspNetUsers_Id_KH];
        PRINT 'Đã xóa FK_DonHangs_AspNetUsers_Id_KH';
    END
    
    -- Kiểm tra xem cột User_Id đã tồn tại chưa
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.DonHangs') AND name = 'User_Id')
    BEGIN
        -- Đổi tên cột Id_KH thành User_Id
        EXEC sp_rename 'dbo.DonHangs.Id_KH', 'User_Id', 'COLUMN';
        PRINT 'Đã đổi tên cột Id_KH thành User_Id trong bảng DonHangs';
    END
    
    -- Tạo lại ràng buộc khóa ngoại cho DonHangs và User_Id
    ALTER TABLE [dbo].[DonHangs] WITH CHECK ADD 
    CONSTRAINT [FK_DonHangs_AspNetUsers] FOREIGN KEY([User_Id])
    REFERENCES [dbo].[AspNetUsers] ([Id])
    ON DELETE NO ACTION;
    PRINT 'Đã tạo FK_DonHangs_AspNetUsers cho User_Id';
    
    COMMIT TRANSACTION;
    PRINT 'Tất cả các thay đổi đã được cập nhật thành công!';
    PRINT 'Lưu ý: Hãy làm mới (refresh) SQL Server Management Studio và tạo lại diagram để thấy các thay đổi';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 