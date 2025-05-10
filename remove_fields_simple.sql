USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Đang xóa các ràng buộc mặc định (default constraints) nếu có...';
    
    -- Xóa default constraint cho các cột cần xóa
    DECLARE @SQL NVARCHAR(MAX) = '';
    DECLARE @DefaultConstraints TABLE (ConstraintName NVARCHAR(128), ColumnName NVARCHAR(128));
    
    -- Tìm tất cả các default constraints trên các cột cần xóa
    INSERT INTO @DefaultConstraints
    SELECT dc.name, c.name
    FROM sys.default_constraints dc
    JOIN sys.columns c ON dc.parent_object_id = c.object_id AND dc.parent_column_id = c.column_id
    WHERE c.name IN ('Id_NguoiTao', 'NgayTao', 'NgayCapNhat')
    AND OBJECT_NAME(dc.parent_object_id) = 'Vouchers';
    
    -- Xóa từng default constraint
    DECLARE @ConstraintName NVARCHAR(128), @ColumnName NVARCHAR(128);
    DECLARE curConstraints CURSOR FOR 
    SELECT ConstraintName, ColumnName FROM @DefaultConstraints;
    
    OPEN curConstraints;
    FETCH NEXT FROM curConstraints INTO @ConstraintName, @ColumnName;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @SQL = 'ALTER TABLE [dbo].[Vouchers] DROP CONSTRAINT ' + QUOTENAME(@ConstraintName);
        EXEC sp_executesql @SQL;
        PRINT 'Đã xóa ràng buộc mặc định ' + @ConstraintName + ' từ cột ' + @ColumnName;
        
        FETCH NEXT FROM curConstraints INTO @ConstraintName, @ColumnName;
    END
    
    CLOSE curConstraints;
    DEALLOCATE curConstraints;
    
    -- Xóa các cột không cần thiết
    PRINT 'Đang xóa các cột không cần thiết...';
    
    PRINT 'Tìm các index có sử dụng các cột cần xóa...';
    DECLARE @IndexesToDrop TABLE (IndexName NVARCHAR(128));
    
    INSERT INTO @IndexesToDrop
    SELECT i.name
    FROM sys.indexes i
    JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
    JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE c.name IN ('Id_NguoiTao', 'NgayTao', 'NgayCapNhat')
    AND OBJECT_NAME(i.object_id) = 'Vouchers'
    AND i.is_primary_key = 0; -- Không xóa primary key
    
    -- Xóa từng index
    DECLARE @IndexName NVARCHAR(128);
    DECLARE curIndexes CURSOR FOR 
    SELECT IndexName FROM @IndexesToDrop;
    
    OPEN curIndexes;
    FETCH NEXT FROM curIndexes INTO @IndexName;
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @SQL = 'DROP INDEX ' + QUOTENAME(@IndexName) + ' ON [dbo].[Vouchers]';
        EXEC sp_executesql @SQL;
        PRINT 'Đã xóa index ' + @IndexName;
        
        FETCH NEXT FROM curIndexes INTO @IndexName;
    END
    
    CLOSE curIndexes;
    DEALLOCATE curIndexes;
    
    -- Xóa các cột
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Vouchers') AND name = 'Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP COLUMN [Id_NguoiTao];
        PRINT 'Đã xóa cột Id_NguoiTao khỏi bảng Vouchers';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Vouchers') AND name = 'NgayTao')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP COLUMN [NgayTao];
        PRINT 'Đã xóa cột NgayTao khỏi bảng Vouchers';
    END
    
    IF EXISTS (SELECT 1 FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Vouchers') AND name = 'NgayCapNhat')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP COLUMN [NgayCapNhat];
        PRINT 'Đã xóa cột NgayCapNhat khỏi bảng Vouchers';
    END
    
    COMMIT TRANSACTION;
    PRINT 'Đã xóa thành công các cột không cần thiết!';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 