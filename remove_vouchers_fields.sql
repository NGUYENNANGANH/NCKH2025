USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Đang xóa các thuộc tính không cần thiết khỏi bảng Vouchers...';
    
    -- 1. Xóa khóa ngoại liên quan đến Id_NguoiTao (nếu có)
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Vouchers_TO_AspNetUsers_Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP CONSTRAINT [FK_Vouchers_TO_AspNetUsers_Id_NguoiTao];
        PRINT 'Đã xóa ràng buộc khóa ngoại FK_Vouchers_TO_AspNetUsers_Id_NguoiTao';
    END
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Vouchers_AspNetUsers')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP CONSTRAINT [FK_Vouchers_AspNetUsers];
        PRINT 'Đã xóa ràng buộc khóa ngoại FK_Vouchers_AspNetUsers';
    END
    
    IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Vouchers_TO_AspNetUsers')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP CONSTRAINT [FK_Vouchers_TO_AspNetUsers];
        PRINT 'Đã xóa ràng buộc khóa ngoại FK_Vouchers_TO_AspNetUsers';
    END
    
    -- 2. Xóa các ràng buộc mặc định (default constraints) nếu có
    DECLARE @SQL NVARCHAR(MAX);
    
    -- Xóa default constraint trên cột Id_NguoiTao
    SELECT @SQL = 'ALTER TABLE [dbo].[Vouchers] DROP CONSTRAINT ' + df.name
    FROM sys.default_constraints df
    JOIN sys.columns c ON df.parent_object_id = c.object_id AND df.parent_column_id = c.column_id
    WHERE c.name = 'Id_NguoiTao' AND OBJECT_NAME(df.parent_object_id) = 'Vouchers';
    
    IF @SQL IS NOT NULL
    BEGIN
        EXEC sp_executesql @SQL;
        PRINT 'Đã xóa ràng buộc mặc định cho cột Id_NguoiTao';
        SET @SQL = NULL;
    END
    
    -- Xóa default constraint trên cột NgayTao
    SELECT @SQL = 'ALTER TABLE [dbo].[Vouchers] DROP CONSTRAINT ' + df.name
    FROM sys.default_constraints df
    JOIN sys.columns c ON df.parent_object_id = c.object_id AND df.parent_column_id = c.column_id
    WHERE c.name = 'NgayTao' AND OBJECT_NAME(df.parent_object_id) = 'Vouchers';
    
    IF @SQL IS NOT NULL
    BEGIN
        EXEC sp_executesql @SQL;
        PRINT 'Đã xóa ràng buộc mặc định cho cột NgayTao';
        SET @SQL = NULL;
    END
    
    -- Xóa default constraint trên cột NgayCapNhat
    SELECT @SQL = 'ALTER TABLE [dbo].[Vouchers] DROP CONSTRAINT ' + df.name
    FROM sys.default_constraints df
    JOIN sys.columns c ON df.parent_object_id = c.object_id AND df.parent_column_id = c.column_id
    WHERE c.name = 'NgayCapNhat' AND OBJECT_NAME(df.parent_object_id) = 'Vouchers';
    
    IF @SQL IS NOT NULL
    BEGIN
        EXEC sp_executesql @SQL;
        PRINT 'Đã xóa ràng buộc mặc định cho cột NgayCapNhat';
        SET @SQL = NULL;
    END
    
    -- 3. Xóa các cột không cần thiết
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Vouchers') AND name = 'Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP COLUMN [Id_NguoiTao];
        PRINT 'Đã xóa cột Id_NguoiTao khỏi bảng Vouchers';
    END
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Vouchers') AND name = 'NgayTao')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP COLUMN [NgayTao];
        PRINT 'Đã xóa cột NgayTao khỏi bảng Vouchers';
    END
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Vouchers') AND name = 'NgayCapNhat')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP COLUMN [NgayCapNhat];
        PRINT 'Đã xóa cột NgayCapNhat khỏi bảng Vouchers';
    END
    
    COMMIT TRANSACTION;
    PRINT 'Đã xóa thành công các thuộc tính Id_NguoiTao, NgayTao, NgayCapNhat khỏi bảng Vouchers!';
    PRINT 'Lưu ý: Hãy làm mới (refresh) SQL Server Management Studio để thấy các thay đổi';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 