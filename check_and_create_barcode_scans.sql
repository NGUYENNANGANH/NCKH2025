USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- 1. Kiểm tra cột Barcode đã tồn tại chưa
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SanPhams') AND name = 'Barcode')
    BEGIN
        PRINT 'Cột Barcode đã tồn tại trong bảng SanPhams';
    END
    ELSE
    BEGIN
        ALTER TABLE [dbo].[SanPhams] ADD [Barcode] NVARCHAR(50) NULL;
        PRINT 'Đã thêm cột Barcode vào bảng SanPhams';
    END
    
    -- 2. Tạo bảng BarcodeScans để lưu lịch sử quét nếu chưa tồn tại
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeScans]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [dbo].[BarcodeScans] (
            [Id] INT IDENTITY(1,1) PRIMARY KEY,
            [Barcode] NVARCHAR(50) NOT NULL,
            [User_Id] NVARCHAR(450) NULL,
            [ScanDate] DATETIME NOT NULL DEFAULT GETDATE(),
            [FoundInDatabase] BIT NOT NULL,
            [Id_SanPham] NVARCHAR(450) NULL,
            [ApiResponse] NVARCHAR(MAX) NULL,
            [IsAdded] BIT NOT NULL DEFAULT 0,
            [DateAdded] DATETIME NULL
        );
        
        PRINT 'Đã tạo bảng BarcodeScans để lưu lịch sử quét mã barcode';
    END
    ELSE
    BEGIN
        PRINT 'Bảng BarcodeScans đã tồn tại';
    END
    
    -- 3. Kiểm tra các cột trong bảng BarcodeScans
    SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'BarcodeScans';
    
    -- 4. Kiểm tra barcode trong bảng SanPhams
    SELECT TOP 10 [Id_SanPham], [TenSanPham], [Barcode]
    FROM SanPhams
    WHERE Barcode IS NOT NULL;
    
    COMMIT TRANSACTION;
    PRINT 'Đã hoàn thành kiểm tra và cập nhật hỗ trợ mã barcode!';
    
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 