USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Đang thêm hỗ trợ mã barcode cho hệ thống...';
    
    -- 1. Thêm cột Barcode vào bảng SanPhams nếu chưa tồn tại
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SanPhams') AND name = 'Barcode')
    BEGIN
        ALTER TABLE [dbo].[SanPhams] ADD [Barcode] NVARCHAR(50) NULL;
        CREATE INDEX [IX_SanPhams_Barcode] ON [dbo].[SanPhams] ([Barcode]);
        PRINT 'Đã thêm cột Barcode vào bảng SanPhams và tạo index';
    END
    ELSE
    BEGIN
        PRINT 'Cột Barcode đã tồn tại trong bảng SanPhams';
    END
    
    -- 2. Tạo bảng BarcodeScans để lưu lịch sử quét
    IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeScans]') AND type in (N'U'))
    BEGIN
        CREATE TABLE [dbo].[BarcodeScans] (
            [Id] INT IDENTITY(1,1) PRIMARY KEY,
            [Barcode] NVARCHAR(50) NOT NULL,
            [User_Id] NVARCHAR(450) NULL,  -- Id người dùng thực hiện quét
            [ScanDate] DATETIME NOT NULL DEFAULT GETDATE(),
            [FoundInDatabase] BIT NOT NULL,  -- Có tìm thấy trong database không
            [Id_SanPham] NVARCHAR(450) NULL,  -- Id sản phẩm nếu tìm thấy
            [ApiResponse] NVARCHAR(MAX) NULL,  -- Lưu response từ API
            [IsAdded] BIT NOT NULL DEFAULT 0,  -- Đã thêm vào database chưa
            [DateAdded] DATETIME NULL  -- Ngày thêm vào database
        );
        
        -- Tạo index cho các cột tìm kiếm thường xuyên
        CREATE INDEX [IX_BarcodeScans_Barcode] ON [dbo].[BarcodeScans] ([Barcode]);
        CREATE INDEX [IX_BarcodeScans_User_Id] ON [dbo].[BarcodeScans] ([User_Id]);
        CREATE INDEX [IX_BarcodeScans_ScanDate] ON [dbo].[BarcodeScans] ([ScanDate]);
        
        -- Tạo foreign key đến bảng Users
        ALTER TABLE [dbo].[BarcodeScans] WITH CHECK ADD 
        CONSTRAINT [FK_BarcodeScans_AspNetUsers] FOREIGN KEY([User_Id])
        REFERENCES [dbo].[AspNetUsers] ([Id]);
        
        -- Tạo foreign key đến bảng SanPhams (nếu tìm thấy sản phẩm)
        ALTER TABLE [dbo].[BarcodeScans] WITH CHECK ADD 
        CONSTRAINT [FK_BarcodeScans_SanPhams] FOREIGN KEY([Id_SanPham])
        REFERENCES [dbo].[SanPhams] ([Id_SanPham]);
        
        PRINT 'Đã tạo bảng BarcodeScans để lưu lịch sử quét mã barcode';
    END
    ELSE
    BEGIN
        PRINT 'Bảng BarcodeScans đã tồn tại';
    END
    
    -- 3. Tạo Stored Procedure để lưu lịch sử quét barcode
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
    PRINT 'Đã tạo stored procedure sp_RecordBarcodeScan để ghi lại lịch sử quét';
    
    -- 4. Tạo Stored Procedure để cập nhật thông tin thêm vào database
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
    PRINT 'Đã tạo stored procedure sp_UpdateBarcodeScanAdded để cập nhật khi thêm vào database';
    
    COMMIT TRANSACTION;
    PRINT '===============================================================';
    PRINT 'Đã thêm thành công hỗ trợ mã barcode cho hệ thống!';
    PRINT '1. Đã thêm cột Barcode vào bảng SanPhams';
    PRINT '2. Đã tạo bảng BarcodeScans để lưu lịch sử quét';
    PRINT '3. Đã tạo stored procedures để quản lý quét barcode';
    PRINT '===============================================================';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 