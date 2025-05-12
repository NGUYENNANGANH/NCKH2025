USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Xóa dữ liệu trong bảng BarcodeScans để có thể thay đổi kiểu dữ liệu
    TRUNCATE TABLE [dbo].[BarcodeScans];
    PRINT 'Đã xóa dữ liệu trong bảng BarcodeScans';
    
    -- Sửa kiểu dữ liệu của cột Id_SanPham
    ALTER TABLE [dbo].[BarcodeScans] ALTER COLUMN [Id_SanPham] INT NULL;
    PRINT 'Đã sửa kiểu dữ liệu của cột Id_SanPham thành INT';
    
    -- Thêm foreign key SanPhams
    ALTER TABLE [dbo].[BarcodeScans] WITH CHECK ADD 
    CONSTRAINT [FK_BarcodeScans_SanPhams] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham]);
    PRINT 'Đã thêm Foreign Key FK_BarcodeScans_SanPhams';
    
    -- Thêm foreign key AspNetUsers
    ALTER TABLE [dbo].[BarcodeScans] WITH CHECK ADD 
    CONSTRAINT [FK_BarcodeScans_AspNetUsers] FOREIGN KEY([User_Id])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Đã thêm Foreign Key FK_BarcodeScans_AspNetUsers';
    
    -- Thêm các chỉ mục cần thiết
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeScans]') AND name = 'IX_BarcodeScans_Barcode')
    BEGIN
        CREATE INDEX [IX_BarcodeScans_Barcode] ON [dbo].[BarcodeScans] ([Barcode]);
        PRINT 'Đã tạo Index IX_BarcodeScans_Barcode';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeScans]') AND name = 'IX_BarcodeScans_User_Id')
    BEGIN
        CREATE INDEX [IX_BarcodeScans_User_Id] ON [dbo].[BarcodeScans] ([User_Id]);
        PRINT 'Đã tạo Index IX_BarcodeScans_User_Id';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeScans]') AND name = 'IX_BarcodeScans_Id_SanPham')
    BEGIN
        CREATE INDEX [IX_BarcodeScans_Id_SanPham] ON [dbo].[BarcodeScans] ([Id_SanPham]);
        PRINT 'Đã tạo Index IX_BarcodeScans_Id_SanPham';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SanPhams]') AND name = 'IX_SanPhams_Barcode')
    BEGIN
        CREATE INDEX [IX_SanPhams_Barcode] ON [dbo].[SanPhams] ([Barcode]);
        PRINT 'Đã tạo Index IX_SanPhams_Barcode';
    END
    
    -- Cập nhật stored procedures
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
        @Id_SanPham INT = NULL,
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
    PRINT 'Đã tạo stored procedure sp_RecordBarcodeScan với kiểu dữ liệu mới';
    
    IF EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[sp_UpdateBarcodeScanAdded]') AND type in (N'P'))
    BEGIN
        DROP PROCEDURE [dbo].[sp_UpdateBarcodeScanAdded];
        PRINT 'Đã xóa stored procedure sp_UpdateBarcodeScanAdded cũ';
    END
    
    EXEC('
    CREATE PROCEDURE [dbo].[sp_UpdateBarcodeScanAdded]
        @ScanId INT,
        @Id_SanPham INT
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
    PRINT 'Đã tạo stored procedure sp_UpdateBarcodeScanAdded với kiểu dữ liệu mới';
    
    -- Kiểm tra lại cấu trúc bảng
    SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE
    FROM INFORMATION_SCHEMA.COLUMNS
    WHERE TABLE_NAME = 'BarcodeScans'
    ORDER BY ORDINAL_POSITION;
    
    -- Kiểm tra foreign keys
    SELECT 
        fk.name AS ForeignKeyName,
        OBJECT_NAME(fk.parent_object_id) AS TableName,
        COL_NAME(fkc.parent_object_id, fkc.parent_column_id) AS ColumnName,
        OBJECT_NAME(fk.referenced_object_id) AS ReferencedTableName,
        COL_NAME(fkc.referenced_object_id, fkc.referenced_column_id) AS ReferencedColumnName
    FROM 
        sys.foreign_keys AS fk
    INNER JOIN 
        sys.foreign_key_columns AS fkc ON fk.object_id = fkc.constraint_object_id
    WHERE 
        OBJECT_NAME(fk.parent_object_id) = 'BarcodeScans';
    
    COMMIT TRANSACTION;
    PRINT '===============================================================';
    PRINT 'Đã sửa thành công cấu trúc bảng BarcodeScans và thêm foreign keys!';
    PRINT '===============================================================';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 