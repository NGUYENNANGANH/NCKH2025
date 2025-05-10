USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Đang cập nhật cấu trúc bảng...';
    
    -- 1. Kiểm tra và đổi tên cột Id_KH thành User_Id trong bảng DonHangs
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.DonHangs') AND name = 'Id_KH')
        AND NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.DonHangs') AND name = 'User_Id')
    BEGIN
        -- Xóa khóa ngoại trước khi đổi tên cột
        IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_DonHangs_TO_AspNetUsers_Id_KH')
        BEGIN
            ALTER TABLE [dbo].[DonHangs] DROP CONSTRAINT [FK_DonHangs_TO_AspNetUsers_Id_KH];
            PRINT 'Đã xóa khóa ngoại FK_DonHangs_TO_AspNetUsers_Id_KH';
        END
        
        EXEC sp_rename 'dbo.DonHangs.Id_KH', 'User_Id', 'COLUMN';
        PRINT 'Đã đổi tên cột Id_KH thành User_Id trong bảng DonHangs';
        
        -- Tạo lại khóa ngoại với tên đơn giản hơn
        ALTER TABLE [dbo].[DonHangs] WITH CHECK ADD 
        CONSTRAINT [FK_DonHangs_AspNetUsers] FOREIGN KEY([User_Id])
        REFERENCES [dbo].[AspNetUsers] ([Id])
        ON DELETE NO ACTION;
        PRINT 'Đã tạo khóa ngoại FK_DonHangs_AspNetUsers cho User_Id';
    END
    
    -- 2. Điều chỉnh mối quan hệ trong VoucherSuDungs để hiển thị đúng trong diagram
    PRINT 'Cập nhật mối quan hệ VoucherSuDungs...';
    
    -- Xóa ràng buộc cũ (nếu có)
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
    
    -- Thêm các ràng buộc mới với định dạng đơn giản hơn
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSuDungs_Vouchers')
    BEGIN
        ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
        CONSTRAINT [FK_VoucherSuDungs_Vouchers] FOREIGN KEY([Id_Voucher])
        REFERENCES [dbo].[Vouchers] ([Id_Voucher]);
        PRINT 'Đã tạo FK_VoucherSuDungs_Vouchers';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSuDungs_DonHangs')
    BEGIN
        ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
        CONSTRAINT [FK_VoucherSuDungs_DonHangs] FOREIGN KEY([Id_DonHang])
        REFERENCES [dbo].[DonHangs] ([Id_DonHang]);
        PRINT 'Đã tạo FK_VoucherSuDungs_DonHangs';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSuDungs_AspNetUsers')
    BEGIN
        ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
        CONSTRAINT [FK_VoucherSuDungs_AspNetUsers] FOREIGN KEY([Id_User])
        REFERENCES [dbo].[AspNetUsers] ([Id]);
        PRINT 'Đã tạo FK_VoucherSuDungs_AspNetUsers';
    END
    
    -- 3. Hướng dẫn người dùng tạo lại diagram
    PRINT '===============================================================';
    PRINT 'Cập nhật cấu trúc bảng thành công!';
    PRINT '- Đã đổi tên Id_KH thành User_Id trong bảng DonHangs';
    PRINT '- Đã cập nhật mối quan hệ để hiển thị đúng trong diagram';
    PRINT '===============================================================';
    PRINT 'Lưu ý: Hãy làm mới (refresh) SQL Server Management Studio và tạo lại diagram để thấy các thay đổi';
    PRINT 'Hãy chạy lệnh sau để xóa các diagram hiện có: DELETE FROM dbo.sysdiagrams WHERE name LIKE ''%''';
    
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 