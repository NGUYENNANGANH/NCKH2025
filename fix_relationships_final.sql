USE [Ban_Hang]
GO

-- Tạo bảng tạm để kiểm tra mối quan hệ cần tạo lại
CREATE TABLE #RelationshipFixes (
    TableFrom NVARCHAR(100),
    TableTo NVARCHAR(100),
    ColumnName NVARCHAR(100),
    ReferencedColumn NVARCHAR(100),
    IsCascade BIT
);

-- Thêm các mối quan hệ cần chuẩn hóa hoặc tạo mới
INSERT INTO #RelationshipFixes
VALUES
('ChiTietDonHangs', 'DonHangs', 'Id_DonHang', 'Id_DonHang', 1),
('ChiTietDonHangs', 'SanPhams', 'Id_SanPham', 'Id_SanPham', 0),
('ChiTietGioHangs', 'GioHangs', 'Id_GioHang', 'Id_GioHang', 1),
('ChiTietGioHangs', 'SanPhams', 'Id_SanPham', 'Id_SanPham', 0),
('DanhGia', 'SanPhams', 'Id_SanPham', 'Id_SanPham', 1),
('DanhGia', 'AspNetUsers', 'Id_User', 'Id', 0),
('DonHangs', 'AspNetUsers', 'Id_KH', 'Id', 0),
('GioHangs', 'AspNetUsers', 'Id_User', 'Id', 0),
('HoaDons', 'DonHangs', 'Id_DonHang', 'Id_DonHang', 1),
('HoaDons', 'AspNetUsers', 'Id_User', 'Id', 0),
('Payments', 'DonHangs', 'Id_DonHang', 'Id_DonHang', 1),
('Payments', 'AspNetUsers', 'Id_User', 'Id', 0),
('SanPham_KhuyenMais', 'KhuyenMais', 'Id_KhuyenMai', 'Id_KhuyenMai', 1),
('SanPham_KhuyenMais', 'SanPhams', 'Id_SanPham', 'Id_SanPham', 1),
('SanPham_KhuyenMais', 'AspNetUsers', 'Id_NguoiTao', 'Id', 0),
('SanPham_KhuyenMais', 'AspNetUsers', 'Id_NguoiCapNhat', 'Id', 0),
('SanPhams', 'DanhMucs', 'Id_DanhMuc', 'Id_DanhMuc', 0),
('VoucherDanhMucs', 'DanhMucs', 'Id_DanhMuc', 'Id_DanhMuc', 1),
('VoucherDanhMucs', 'Vouchers', 'Id_Voucher', 'Id_Voucher', 1),
('VoucherSanPhams', 'SanPhams', 'Id_SanPham', 'Id_SanPham', 1),
('VoucherSanPhams', 'Vouchers', 'Id_Voucher', 'Id_Voucher', 1),
('VoucherSuDungs', 'Vouchers', 'Id_Voucher', 'Id_Voucher', 1),
('VoucherSuDungs', 'DonHangs', 'Id_DonHang', 'Id_DonHang', 1),
('VoucherSuDungs', 'AspNetUsers', 'Id_User', 'Id', 0),
('Vouchers', 'AspNetUsers', 'Id_NguoiTao', 'Id', 0);

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Gỡ bỏ tất cả các ràng buộc không phải Identity Framework
    DECLARE @DropConstraintSQL NVARCHAR(MAX) = '';
    
    SELECT @DropConstraintSQL = @DropConstraintSQL + 
        'ALTER TABLE [dbo].[' + OBJECT_NAME(parent_object_id) + '] ' +
        'DROP CONSTRAINT [' + name + '];' + CHAR(13) + CHAR(10)
    FROM sys.foreign_keys
    WHERE name NOT LIKE 'FK_AspNet%'
    ORDER BY name;
    
    PRINT 'Removing all current foreign keys...';
    EXEC sp_executesql @DropConstraintSQL;
    
    -- Tạo tất cả các ràng buộc mới với định dạng nhất quán
    DECLARE @TableFrom NVARCHAR(100)
    DECLARE @TableTo NVARCHAR(100)
    DECLARE @ColumnName NVARCHAR(100)
    DECLARE @ReferencedColumn NVARCHAR(100)
    DECLARE @IsCascade BIT
    DECLARE @SQL NVARCHAR(MAX)
    
    DECLARE relationship_cursor CURSOR FOR 
    SELECT TableFrom, TableTo, ColumnName, ReferencedColumn, IsCascade FROM #RelationshipFixes;
    
    OPEN relationship_cursor
    
    FETCH NEXT FROM relationship_cursor INTO @TableFrom, @TableTo, @ColumnName, @ReferencedColumn, @IsCascade
    
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @SQL = 'ALTER TABLE [dbo].[' + @TableFrom + '] WITH CHECK ADD ' +
                   'CONSTRAINT [FK_' + @TableFrom + '_TO_' + @TableTo + CASE WHEN @ColumnName <> 'Id' + @TableTo THEN '_' + @ColumnName ELSE '' END + '] ' +
                   'FOREIGN KEY([' + @ColumnName + ']) ' +
                   'REFERENCES [dbo].[' + @TableTo + '] ([' + @ReferencedColumn + '])' +
                   CASE WHEN @IsCascade = 1 THEN ' ON DELETE CASCADE' ELSE '' END;
                   
        EXEC sp_executesql @SQL;
        
        PRINT 'Added FK_' + @TableFrom + '_TO_' + @TableTo + CASE WHEN @ColumnName <> 'Id' + @TableTo THEN '_' + @ColumnName ELSE '' END;
        
        FETCH NEXT FROM relationship_cursor INTO @TableFrom, @TableTo, @ColumnName, @ReferencedColumn, @IsCascade
    END
    
    CLOSE relationship_cursor;
    DEALLOCATE relationship_cursor;
    
    -- Xóa bảng tạm
    DROP TABLE #RelationshipFixes;
    
    COMMIT TRANSACTION;
    PRINT 'All foreign key relationships have been successfully standardized!';
END TRY
BEGIN CATCH
    IF (CURSOR_STATUS('global','relationship_cursor') >= 0)
    BEGIN
        CLOSE relationship_cursor;
        DEALLOCATE relationship_cursor;
    END
    
    IF OBJECT_ID('tempdb..#RelationshipFixes') IS NOT NULL 
        DROP TABLE #RelationshipFixes;
        
    ROLLBACK TRANSACTION;
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH 