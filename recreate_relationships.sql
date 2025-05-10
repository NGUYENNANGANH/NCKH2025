USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Xóa các ràng buộc hiện có liên quan đến DanhMuc và VoucherDanhMuc
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherDanhMucs_DanhMucs')
    BEGIN
        ALTER TABLE [dbo].[VoucherDanhMucs] DROP CONSTRAINT [FK_VoucherDanhMucs_DanhMucs];
        PRINT 'Removed FK_VoucherDanhMucs_DanhMucs';
    END
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SanPhams_DanhMucs')
    BEGIN
        ALTER TABLE [dbo].[SanPhams] DROP CONSTRAINT [FK_SanPhams_DanhMucs];
        PRINT 'Removed FK_SanPhams_DanhMucs';
    END
    
    -- Tạo lại ràng buộc với tên mới và rõ ràng hơn
    ALTER TABLE [dbo].[VoucherDanhMucs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherDanhMucs_TO_DanhMucs] FOREIGN KEY([Id_DanhMuc])
    REFERENCES [dbo].[DanhMucs] ([Id_DanhMuc]);
    PRINT 'Created FK_VoucherDanhMucs_TO_DanhMucs';
    
    ALTER TABLE [dbo].[SanPhams] WITH CHECK ADD 
    CONSTRAINT [FK_SanPhams_TO_DanhMucs] FOREIGN KEY([Id_DanhMuc])
    REFERENCES [dbo].[DanhMucs] ([Id_DanhMuc]);
    PRINT 'Created FK_SanPhams_TO_DanhMucs';
    
    COMMIT TRANSACTION;
    PRINT 'All relationships have been recreated successfully!';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH 