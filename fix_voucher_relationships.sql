USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Đang xóa các mối quan hệ hiện tại của bảng VoucherSuDungs...';
    
    -- Xóa các ràng buộc khóa ngoại hiện có trên bảng VoucherSuDungs
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
    
    -- Tạo lại các ràng buộc khóa ngoại với cấu trúc rõ ràng hơn cho bảng VoucherSuDungs
    PRINT 'Đang tạo lại các mối quan hệ khóa ngoại...';
    
    -- 1. VoucherSuDungs -> Vouchers
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_Vouchers] FOREIGN KEY([Id_Voucher])
    REFERENCES [dbo].[Vouchers] ([Id_Voucher])
    ON DELETE CASCADE;
    PRINT 'Đã tạo FK_VoucherSuDungs_Vouchers';
    
    -- 2. VoucherSuDungs -> DonHangs
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang])
    ON DELETE CASCADE;
    PRINT 'Đã tạo FK_VoucherSuDungs_DonHangs';
    
    -- 3. VoucherSuDungs -> AspNetUsers
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id])
    ON DELETE NO ACTION;
    PRINT 'Đã tạo FK_VoucherSuDungs_AspNetUsers';
    
    COMMIT TRANSACTION;
    PRINT 'Tất cả các mối quan hệ khóa ngoại của bảng VoucherSuDungs đã được cập nhật thành công!';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 