USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Đang thêm các ràng buộc khóa ngoại cho bảng BarcodeScans...';
    
    -- 1. Thêm Foreign Key liên kết đến bảng SanPhams (BarcodeScans.Id_SanPham -> SanPhams.Id_SanPham)
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BarcodeScans_SanPhams]') AND parent_object_id = OBJECT_ID(N'[dbo].[BarcodeScans]'))
    BEGIN
        ALTER TABLE [dbo].[BarcodeScans] WITH CHECK ADD 
        CONSTRAINT [FK_BarcodeScans_SanPhams] FOREIGN KEY([Id_SanPham])
        REFERENCES [dbo].[SanPhams] ([Id_SanPham]);
        
        PRINT 'Đã thêm Foreign Key FK_BarcodeScans_SanPhams';
    END
    ELSE
    BEGIN
        PRINT 'Foreign Key FK_BarcodeScans_SanPhams đã tồn tại';
    END
    
    -- 2. Thêm Foreign Key liên kết đến bảng AspNetUsers (BarcodeScans.User_Id -> AspNetUsers.Id)
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_BarcodeScans_AspNetUsers]') AND parent_object_id = OBJECT_ID(N'[dbo].[BarcodeScans]'))
    BEGIN
        ALTER TABLE [dbo].[BarcodeScans] WITH CHECK ADD 
        CONSTRAINT [FK_BarcodeScans_AspNetUsers] FOREIGN KEY([User_Id])
        REFERENCES [dbo].[AspNetUsers] ([Id]);
        
        PRINT 'Đã thêm Foreign Key FK_BarcodeScans_AspNetUsers';
    END
    ELSE
    BEGIN
        PRINT 'Foreign Key FK_BarcodeScans_AspNetUsers đã tồn tại';
    END
    
    -- 3. Thêm Index cho cột Barcode trên bảng SanPhams nếu chưa có
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[SanPhams]') AND name = 'IX_SanPhams_Barcode')
    BEGIN
        CREATE INDEX [IX_SanPhams_Barcode] ON [dbo].[SanPhams] ([Barcode]);
        PRINT 'Đã tạo Index IX_SanPhams_Barcode';
    END
    ELSE
    BEGIN
        PRINT 'Index IX_SanPhams_Barcode đã tồn tại';
    END
    
    -- 4. Thêm các Index cho bảng BarcodeScans
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeScans]') AND name = 'IX_BarcodeScans_Barcode')
    BEGIN
        CREATE INDEX [IX_BarcodeScans_Barcode] ON [dbo].[BarcodeScans] ([Barcode]);
        PRINT 'Đã tạo Index IX_BarcodeScans_Barcode';
    END
    ELSE
    BEGIN
        PRINT 'Index IX_BarcodeScans_Barcode đã tồn tại';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeScans]') AND name = 'IX_BarcodeScans_User_Id')
    BEGIN
        CREATE INDEX [IX_BarcodeScans_User_Id] ON [dbo].[BarcodeScans] ([User_Id]);
        PRINT 'Đã tạo Index IX_BarcodeScans_User_Id';
    END
    ELSE
    BEGIN
        PRINT 'Index IX_BarcodeScans_User_Id đã tồn tại';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeScans]') AND name = 'IX_BarcodeScans_Id_SanPham')
    BEGIN
        CREATE INDEX [IX_BarcodeScans_Id_SanPham] ON [dbo].[BarcodeScans] ([Id_SanPham]);
        PRINT 'Đã tạo Index IX_BarcodeScans_Id_SanPham';
    END
    ELSE
    BEGIN
        PRINT 'Index IX_BarcodeScans_Id_SanPham đã tồn tại';
    END
    
    IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[BarcodeScans]') AND name = 'IX_BarcodeScans_ScanDate')
    BEGIN
        CREATE INDEX [IX_BarcodeScans_ScanDate] ON [dbo].[BarcodeScans] ([ScanDate]);
        PRINT 'Đã tạo Index IX_BarcodeScans_ScanDate';
    END
    ELSE
    BEGIN
        PRINT 'Index IX_BarcodeScans_ScanDate đã tồn tại';
    END
    
    -- 5. Kiểm tra mối quan hệ
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
    PRINT 'Đã thêm thành công các mối quan hệ khóa ngoại cho bảng BarcodeScans!';
    PRINT '1. Đã liên kết BarcodeScans với bảng SanPhams';
    PRINT '2. Đã liên kết BarcodeScans với bảng AspNetUsers';
    PRINT '3. Đã tạo các Index cho việc tìm kiếm hiệu quả';
    PRINT '===============================================================';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 