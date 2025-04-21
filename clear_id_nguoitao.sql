USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;

    -- Tạo bảng tạm để lưu trữ danh sách các ràng buộc cần xóa
    CREATE TABLE #ConstraintsToRemove (
        TableName nvarchar(128),
        ConstraintName nvarchar(128)
    );

    -- Tìm tất cả các ràng buộc liên quan đến Id_NguoiTao
    INSERT INTO #ConstraintsToRemove
    SELECT 
        OBJECT_NAME(parent_object_id) AS TableName,
        name AS ConstraintName
    FROM sys.foreign_keys
    WHERE referenced_column_id IN (
        SELECT column_id FROM sys.columns 
        WHERE name = 'Id_NguoiTao' AND object_id IN (
            SELECT object_id FROM sys.tables WHERE name IN ('SanPham_KhuyenMais', 'Vouchers')
        )
    )
    OR parent_column_id IN (
        SELECT column_id FROM sys.columns 
        WHERE name = 'Id_NguoiTao' AND object_id IN (
            SELECT object_id FROM sys.tables WHERE name IN ('SanPham_KhuyenMais', 'Vouchers')
        )
    );

    -- Tìm thêm các ràng buộc khác nếu có (default, check, etc.)
    INSERT INTO #ConstraintsToRemove
    SELECT 
        OBJECT_NAME(parent_object_id) AS TableName,
        name AS ConstraintName
    FROM sys.default_constraints
    WHERE parent_column_id IN (
        SELECT column_id FROM sys.columns 
        WHERE name IN ('Id_NguoiTao', 'Id_NguoiCapNhat') 
        AND object_id IN (SELECT object_id FROM sys.tables WHERE name IN ('SanPham_KhuyenMais', 'Vouchers'))
    );

    -- Xóa tất cả các ràng buộc đã tìm thấy
    DECLARE @TableName nvarchar(128);
    DECLARE @ConstraintName nvarchar(128);
    DECLARE @SQL nvarchar(max);

    DECLARE constraint_cursor CURSOR FOR 
    SELECT TableName, ConstraintName FROM #ConstraintsToRemove;

    OPEN constraint_cursor;
    FETCH NEXT FROM constraint_cursor INTO @TableName, @ConstraintName;

    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @SQL = 'ALTER TABLE [dbo].[' + @TableName + '] DROP CONSTRAINT [' + @ConstraintName + ']';
        EXEC sp_executesql @SQL;
        PRINT 'Đã xóa ràng buộc ' + @ConstraintName + ' từ bảng ' + @TableName;
        
        FETCH NEXT FROM constraint_cursor INTO @TableName, @ConstraintName;
    END

    CLOSE constraint_cursor;
    DEALLOCATE constraint_cursor;

    -- Xóa bảng tạm
    DROP TABLE #ConstraintsToRemove;

    -- Bây giờ thử xóa các cột không sử dụng
    PRINT 'Đang xóa các cột không sử dụng...';
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SanPham_KhuyenMais') AND name = 'Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP COLUMN [Id_NguoiTao];
        PRINT 'Đã xóa cột Id_NguoiTao khỏi bảng SanPham_KhuyenMais';
    END
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.SanPham_KhuyenMais') AND name = 'Id_NguoiCapNhat')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP COLUMN [Id_NguoiCapNhat];
        PRINT 'Đã xóa cột Id_NguoiCapNhat khỏi bảng SanPham_KhuyenMais';
    END
    
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.Vouchers') AND name = 'Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP COLUMN [Id_NguoiTao];
        PRINT 'Đã xóa cột Id_NguoiTao khỏi bảng Vouchers';
    END

    COMMIT TRANSACTION;
    PRINT 'Tất cả các cột không sử dụng đã được xóa thành công!';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH

-- Kiểm tra kết quả
SELECT 
    TABLE_NAME, COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE COLUMN_NAME IN ('Id_NguoiTao', 'Id_NguoiCapNhat', 'User_Id') 
ORDER BY TABLE_NAME, COLUMN_NAME; 