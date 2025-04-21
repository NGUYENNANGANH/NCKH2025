USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Đang cập nhật cấu trúc bảng...';
    
    -- 1. Xóa tất cả các khóa ngoại có liên quan
    PRINT '-- Xóa các ràng buộc khóa ngoại --';
    
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
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DonHangs_AspNetUsers')
    BEGIN
        ALTER TABLE [dbo].[DonHangs] DROP CONSTRAINT [FK_DonHangs_AspNetUsers];
        PRINT 'Đã xóa FK_DonHangs_AspNetUsers';
    END
    
    -- 2. Xóa các cột không sử dụng
    PRINT '-- Xóa các cột không sử dụng --';
    
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
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Vouchers') AND name = 'Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP COLUMN [Id_NguoiTao];
        PRINT 'Đã xóa cột Id_NguoiTao khỏi bảng Vouchers';
    END
    
    -- 3. Đổi tên cột Id_KH thành User_Id trong bảng DonHangs
    PRINT '-- Đổi tên cột --';
    
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.DonHangs') AND name = 'User_Id')
        AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.DonHangs') AND name = 'Id_KH')
    BEGIN
        EXEC sp_rename 'dbo.DonHangs.Id_KH', 'User_Id', 'COLUMN';
        PRINT 'Đã đổi tên cột Id_KH thành User_Id trong bảng DonHangs';
    END
    
    -- 4. Tạo lại khóa ngoại cho DonHangs.User_Id
    PRINT '-- Tạo lại các ràng buộc khóa ngoại --';
    
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_DonHangs_AspNetUsers')
    BEGIN
        ALTER TABLE [dbo].[DonHangs] WITH CHECK ADD 
        CONSTRAINT [FK_DonHangs_AspNetUsers] FOREIGN KEY([User_Id])
        REFERENCES [dbo].[AspNetUsers] ([Id])
        ON DELETE NO ACTION;
        PRINT 'Đã tạo FK_DonHangs_AspNetUsers cho User_Id';
    END
    
    -- 5. Đặt lại các mối quan hệ trong VoucherSuDungs để hiển thị đúng trong diagram
    PRINT '-- Cập nhật mối quan hệ VoucherSuDungs --';
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSuDungs_TO_Vouchers_Id_Voucher')
    BEGIN
        ALTER TABLE [dbo].[VoucherSuDungs] DROP CONSTRAINT [FK_VoucherSuDungs_TO_Vouchers_Id_Voucher];
        PRINT 'Đã xóa FK_VoucherSuDungs_TO_Vouchers_Id_Voucher';
    END
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSuDungs_TO_DonHangs_Id_DonHang')
    BEGIN
        ALTER TABLE [dbo].[VoucherSuDungs] DROP CONSTRAINT [FK_VoucherSuDungs_TO_DonHangs_Id_DonHang];
        PRINT 'Đã xóa FK_VoucherSuDungs_TO_DonHangs_Id_DonHang';
    END
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSuDungs_TO_AspNetUsers_Id_User')
    BEGIN
        ALTER TABLE [dbo].[VoucherSuDungs] DROP CONSTRAINT [FK_VoucherSuDungs_TO_AspNetUsers_Id_User];
        PRINT 'Đã xóa FK_VoucherSuDungs_TO_AspNetUsers_Id_User';
    END
    
    -- Tạo lại các khóa ngoại với tên rõ ràng hơn
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_Vouchers] FOREIGN KEY([Id_Voucher])
    REFERENCES [dbo].[Vouchers] ([Id_Voucher])
    ON DELETE CASCADE;
    PRINT 'Đã tạo FK_VoucherSuDungs_Vouchers';
    
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang])
    ON DELETE CASCADE;
    PRINT 'Đã tạo FK_VoucherSuDungs_DonHangs';
    
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id])
    ON DELETE NO ACTION;
    PRINT 'Đã tạo FK_VoucherSuDungs_AspNetUsers';
    
    COMMIT TRANSACTION;
    PRINT '===============================================================';
    PRINT 'Tất cả các thay đổi đã được hoàn tất thành công!';
    PRINT '- Đã xóa các cột không sử dụng (Id_NguoiTao, Id_NguoiCapNhat)';
    PRINT '- Đã đổi tên Id_KH thành User_Id trong bảng DonHangs';
    PRINT '- Đã cập nhật mối quan hệ để hiển thị đúng trong diagram';
    PRINT '===============================================================';
    PRINT 'Lưu ý: Hãy làm mới (refresh) SQL Server Management Studio và tạo lại diagram để thấy các thay đổi';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 