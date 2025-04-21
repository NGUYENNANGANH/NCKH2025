USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- 1. Thêm cột Barcode vào bảng SanPhams
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SanPhams') AND name = 'Barcode')
    BEGIN
        ALTER TABLE [dbo].[SanPhams] ADD [Barcode] NVARCHAR(50) NULL;
        PRINT 'Đã thêm cột Barcode vào bảng SanPhams';
    END
    ELSE
    BEGIN
        PRINT 'Cột Barcode đã tồn tại trong bảng SanPhams';
    END
    
    -- 2. Tạo bảng BarcodeScans đơn giản để lưu lịch sử quét
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeScans]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [dbo].[BarcodeScans] (
            [Id] INT IDENTITY(1,1) PRIMARY KEY,
            [Barcode] NVARCHAR(50) NOT NULL,
            [User_Id] NVARCHAR(450) NULL,
            [ScanDate] DATETIME NOT NULL DEFAULT GETDATE(),
            [FoundInDatabase] BIT NOT NULL,
            [Id_SanPham] NVARCHAR(450) NULL,
            [ApiResponse] NVARCHAR(MAX) NULL
        );
        
        PRINT 'Đã tạo bảng BarcodeScans để lưu lịch sử quét mã barcode';
    END
    ELSE
    BEGIN
        PRINT 'Bảng BarcodeScans đã tồn tại';
    END
    
    COMMIT TRANSACTION;
    PRINT 'Đã thêm thành công hỗ trợ mã barcode cơ bản cho hệ thống!';
    
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 