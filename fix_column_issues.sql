USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Đang cập nhật cấu trúc bảng...';
    
    -- 1. Xóa tất cả các mối quan hệ có liên quan đến các cột cần xóa
    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP CONSTRAINT [FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiTao];
        PRINT 'Đã xóa khóa ngoại FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiTao';
    END
    
    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiCapNhat')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP CONSTRAINT [FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiCapNhat];
        PRINT 'Đã xóa khóa ngoại FK_SanPham_KhuyenMais_TO_AspNetUsers_Id_NguoiCapNhat';
    END
    
    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_Vouchers_TO_AspNetUsers_Id_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] DROP CONSTRAINT [FK_Vouchers_TO_AspNetUsers_Id_NguoiTao];
        PRINT 'Đã xóa khóa ngoại FK_Vouchers_TO_AspNetUsers_Id_NguoiTao';
    END
    
    -- 2. Xóa các ràng buộc mặc định (default constraints) nếu có
    DECLARE @SQL NVARCHAR(MAX);
    
    -- Xóa default constraint trên cột Id_NguoiTao của bảng SanPham_KhuyenMais
    SELECT @SQL = 'ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP CONSTRAINT ' + df.name
    FROM sys.default_constraints df
    JOIN sys.columns c ON df.parent_object_id = c.object_id AND df.parent_column_id = c.column_id
    WHERE c.name = 'Id_NguoiTao' AND OBJECT_NAME(df.parent_object_id) = 'SanPham_KhuyenMais';
    
    IF @SQL IS NOT NULL
    BEGIN
        EXEC sp_executesql @SQL;
        PRINT 'Đã xóa default constraint cho cột Id_NguoiTao của bảng SanPham_KhuyenMais';
        SET @SQL = NULL;
    END
    
    -- Xóa default constraint trên cột Id_NguoiCapNhat của bảng SanPham_KhuyenMais
    SELECT @SQL = 'ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP CONSTRAINT ' + df.name
    FROM sys.default_constraints df
    JOIN sys.columns c ON df.parent_object_id = c.object_id AND df.parent_column_id = c.column_id
    WHERE c.name = 'Id_NguoiCapNhat' AND OBJECT_NAME(df.parent_object_id) = 'SanPham_KhuyenMais';
    
    IF @SQL IS NOT NULL
    BEGIN
        EXEC sp_executesql @SQL;
        PRINT 'Đã xóa default constraint cho cột Id_NguoiCapNhat của bảng SanPham_KhuyenMais';
        SET @SQL = NULL;
    END
    
    -- Xóa default constraint trên cột Id_NguoiTao của bảng Vouchers
    SELECT @SQL = 'ALTER TABLE [dbo].[Vouchers] DROP CONSTRAINT ' + df.name
    FROM sys.default_constraints df
    JOIN sys.columns c ON df.parent_object_id = c.object_id AND df.parent_column_id = c.column_id
    WHERE c.name = 'Id_NguoiTao' AND OBJECT_NAME(df.parent_object_id) = 'Vouchers';
    
    IF @SQL IS NOT NULL
    BEGIN
        EXEC sp_executesql @SQL;
        PRINT 'Đã xóa default constraint cho cột Id_NguoiTao của bảng Vouchers';
        SET @SQL = NULL;
    END
    
    -- 3. Xóa các cột không sử dụng
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
    
    -- 4. Đảm bảo cột Id_KH trong bảng DonHangs đã được đổi tên thành User_Id
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.DonHangs') AND name = 'User_Id')
        AND EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.DonHangs') AND name = 'Id_KH')
    BEGIN
        -- Xóa khóa ngoại trước khi đổi tên cột
        IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_DonHangs_TO_AspNetUsers_Id_KH')
        BEGIN
            ALTER TABLE [dbo].[DonHangs] DROP CONSTRAINT [FK_DonHangs_TO_AspNetUsers_Id_KH];
            PRINT 'Đã xóa khóa ngoại FK_DonHangs_TO_AspNetUsers_Id_KH';
        END
        
        EXEC sp_rename 'dbo.DonHangs.Id_KH', 'User_Id', 'COLUMN';
        PRINT 'Đã đổi tên cột Id_KH thành User_Id trong bảng DonHangs';
        
        -- Tạo lại khóa ngoại
        ALTER TABLE [dbo].[DonHangs] WITH CHECK ADD 
        CONSTRAINT [FK_DonHangs_AspNetUsers] FOREIGN KEY([User_Id])
        REFERENCES [dbo].[AspNetUsers] ([Id])
        ON DELETE NO ACTION;
        PRINT 'Đã tạo lại khóa ngoại FK_DonHangs_AspNetUsers';
    END
    
    COMMIT TRANSACTION;
    PRINT 'Cập nhật cấu trúc bảng thành công!';
    
    -- Kiểm tra kết quả
    SELECT TABLE_NAME, COLUMN_NAME 
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME IN ('DonHangs', 'SanPham_KhuyenMais', 'Vouchers')
        AND COLUMN_NAME IN ('Id_NguoiTao', 'Id_NguoiCapNhat', 'User_Id', 'Id_KH')
    ORDER BY TABLE_NAME, COLUMN_NAME;
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 