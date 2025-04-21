USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Thêm cột IsAdded nếu chưa tồn tại
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.BarcodeScans') AND name = 'IsAdded')
    BEGIN
        ALTER TABLE [dbo].[BarcodeScans] ADD [IsAdded] BIT NOT NULL DEFAULT 0;
        PRINT 'Đã thêm cột IsAdded vào bảng BarcodeScans';
    END
    ELSE
    BEGIN
        PRINT 'Cột IsAdded đã tồn tại trong bảng BarcodeScans';
    END
    
    -- Thêm cột DateAdded nếu chưa tồn tại
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.BarcodeScans') AND name = 'DateAdded')
    BEGIN
        ALTER TABLE [dbo].[BarcodeScans] ADD [DateAdded] DATETIME NULL;
        PRINT 'Đã thêm cột DateAdded vào bảng BarcodeScans';
    END
    ELSE
    BEGIN
        PRINT 'Cột DateAdded đã tồn tại trong bảng BarcodeScans';
    END
    
    -- Tạo stored procedure để ghi lại lịch sử quét
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_RecordBarcodeScan]') AND type in (N'P'))
    BEGIN
        DROP PROCEDURE [dbo].[sp_RecordBarcodeScan];
        PRINT 'Đã xóa stored procedure sp_RecordBarcodeScan cũ';
    END
    
    EXEC('
    CREATE PROCEDURE [dbo].[sp_RecordBarcodeScan]
        @Barcode NVARCHAR(50),
        @User_Id NVARCHAR(450) = NULL,
        @FoundInDatabase BIT,
        @Id_SanPham NVARCHAR(450) = NULL,
        @ApiResponse NVARCHAR(MAX) = NULL
    AS
    BEGIN
        INSERT INTO [dbo].[BarcodeScans]
            ([Barcode], [User_Id], [ScanDate], [FoundInDatabase], [Id_SanPham], [ApiResponse])
        VALUES
            (@Barcode, @User_Id, GETDATE(), @FoundInDatabase, @Id_SanPham, @ApiResponse);
            
        SELECT SCOPE_IDENTITY() AS ScanId;
    END
    ');
    PRINT 'Đã tạo stored procedure sp_RecordBarcodeScan';
    
    -- Tạo stored procedure để cập nhật khi thêm sản phẩm vào database
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateBarcodeScanAdded]') AND type in (N'P'))
    BEGIN
        DROP PROCEDURE [dbo].[sp_UpdateBarcodeScanAdded];
        PRINT 'Đã xóa stored procedure sp_UpdateBarcodeScanAdded cũ';
    END
    
    EXEC('
    CREATE PROCEDURE [dbo].[sp_UpdateBarcodeScanAdded]
        @ScanId INT,
        @Id_SanPham NVARCHAR(450)
    AS
    BEGIN
        UPDATE [dbo].[BarcodeScans]
        SET 
            [IsAdded] = 1,
            [DateAdded] = GETDATE(),
            [Id_SanPham] = @Id_SanPham
        WHERE
            [Id] = @ScanId;
    END
    ');
    PRINT 'Đã tạo stored procedure sp_UpdateBarcodeScanAdded';
    
    -- Kiểm tra lại cấu trúc bảng
    SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = 'BarcodeScans'
    ORDER BY ORDINAL_POSITION;
    
    COMMIT TRANSACTION;
    PRINT 'Đã cập nhật thành công cấu trúc bảng BarcodeScans và tạo các stored procedures!';
    
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 