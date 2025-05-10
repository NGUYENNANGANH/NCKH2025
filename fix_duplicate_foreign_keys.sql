USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Xóa các khóa ngoại bị trùng lặp
    
    -- 1. VoucherDanhMucs -> Vouchers (có 2 constraint)
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherDanhMucs_Vouchers' AND 
               EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherDanhMucs_Vouchers_Id_Voucher'))
    BEGIN
        ALTER TABLE [dbo].[VoucherDanhMucs] DROP CONSTRAINT [FK_VoucherDanhMucs_Vouchers];
        PRINT 'Removed duplicate FK_VoucherDanhMucs_Vouchers';
    END
    
    -- 2. VoucherSanPhams -> SanPhams (có 2 constraint)
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSanPhams_SanPhams' AND 
               EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSanPhams_SanPhams_Id_SanPham'))
    BEGIN
        ALTER TABLE [dbo].[VoucherSanPhams] DROP CONSTRAINT [FK_VoucherSanPhams_SanPhams];
        PRINT 'Removed duplicate FK_VoucherSanPhams_SanPhams';
    END
    
    -- 3. VoucherSanPhams -> Vouchers (có 2 constraint)
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSanPhams_Vouchers' AND 
               EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSanPhams_Vouchers_Id_Voucher'))
    BEGIN
        ALTER TABLE [dbo].[VoucherSanPhams] DROP CONSTRAINT [FK_VoucherSanPhams_Vouchers];
        PRINT 'Removed duplicate FK_VoucherSanPhams_Vouchers';
    END
    
    COMMIT TRANSACTION;
    PRINT 'All duplicate foreign keys have been removed successfully!';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH 