USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- 1. Dọn dẹp ràng buộc kép: SanPhams -> DanhMucs
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SanPhams_DanhMucs' AND 
               EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SanPhams_TO_DanhMucs'))
    BEGIN
        ALTER TABLE [dbo].[SanPhams] DROP CONSTRAINT [FK_SanPhams_DanhMucs];
        PRINT 'Removed duplicate FK_SanPhams_DanhMucs';
    END
    
    -- 2. Dọn dẹp ràng buộc kép: VoucherDanhMucs -> DanhMucs
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherDanhMucs_DanhMucs' AND 
               EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherDanhMucs_TO_DanhMucs'))
    BEGIN
        ALTER TABLE [dbo].[VoucherDanhMucs] DROP CONSTRAINT [FK_VoucherDanhMucs_DanhMucs];
        PRINT 'Removed duplicate FK_VoucherDanhMucs_DanhMucs';
    END
    
    COMMIT TRANSACTION;
    PRINT 'All duplicate constraints have been removed!';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH 