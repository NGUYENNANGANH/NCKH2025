USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- 1. Thiếu: ChiTietDonHang -> DonHang (đã được thiết lập với _TO_ nhưng trong diagram vẫn có thể cần sửa)
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChiTietDonHangs_TO_DonHangs')
    BEGIN
        ALTER TABLE [dbo].[ChiTietDonHangs] DROP CONSTRAINT [FK_ChiTietDonHangs_TO_DonHangs];
    END
    
    ALTER TABLE [dbo].[ChiTietDonHangs] WITH CHECK ADD 
    CONSTRAINT [FK_ChiTietDonHangs_TO_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang])
    ON DELETE CASCADE;
    PRINT 'Fixed FK_ChiTietDonHangs_TO_DonHangs';
    
    -- 2. Kiểm tra thực thi ON DELETE CASCADE tất cả các khóa ngoại con-cha
    -- Chưa có ON DELETE CASCADE: HoaDon -> DonHang (nên có CASCADE vì HoaDon là bản ghi phụ thuộc vào DonHang)
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_HoaDons_TO_DonHangs')
    BEGIN
        ALTER TABLE [dbo].[HoaDons] DROP CONSTRAINT [FK_HoaDons_TO_DonHangs];
    END
    
    ALTER TABLE [dbo].[HoaDons] WITH CHECK ADD 
    CONSTRAINT [FK_HoaDons_TO_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang])
    ON DELETE CASCADE;
    PRINT 'Fixed FK_HoaDons_TO_DonHangs with CASCADE';
    
    -- 3. Payment -> DonHang (nên có CASCADE vì Payment phụ thuộc vào DonHang)
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Payments_TO_DonHangs')
    BEGIN
        ALTER TABLE [dbo].[Payments] DROP CONSTRAINT [FK_Payments_TO_DonHangs];
    END
    
    ALTER TABLE [dbo].[Payments] WITH CHECK ADD 
    CONSTRAINT [FK_Payments_TO_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang])
    ON DELETE CASCADE;
    PRINT 'Fixed FK_Payments_TO_DonHangs with CASCADE';
    
    -- 4. VoucherSuDung -> DonHang (nên có CASCADE vì VoucherSuDung phụ thuộc vào DonHang)
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSuDungs_TO_DonHangs')
    BEGIN
        ALTER TABLE [dbo].[VoucherSuDungs] DROP CONSTRAINT [FK_VoucherSuDungs_TO_DonHangs];
    END
    
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_TO_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang])
    ON DELETE CASCADE;
    PRINT 'Fixed FK_VoucherSuDungs_TO_DonHangs with CASCADE';
    
    COMMIT TRANSACTION;
    PRINT 'All missing foreign keys have been added with appropriate cascading relationships!';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH 