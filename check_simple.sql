USE [Ban_Hang]
GO

-- Kiểm tra xem bảng SanPhams có tồn tại không
SELECT 
    CASE 
        WHEN EXISTS (SELECT * FROM sys.tables WHERE name = 'SanPhams') 
        THEN 'Bảng SanPhams tồn tại' 
        ELSE 'Bảng SanPhams KHÔNG tồn tại' 
    END AS KiemTraBangSanPhams;

-- Kiểm tra xem bảng AspNetUsers có tồn tại không
SELECT 
    CASE 
        WHEN EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers') 
        THEN 'Bảng AspNetUsers tồn tại' 
        ELSE 'Bảng AspNetUsers KHÔNG tồn tại' 
    END AS KiemTraBangAspNetUsers;

-- Kiểm tra xem bảng BarcodeScans có tồn tại không
SELECT 
    CASE 
        WHEN EXISTS (SELECT * FROM sys.tables WHERE name = 'BarcodeScans') 
        THEN 'Bảng BarcodeScans tồn tại' 
        ELSE 'Bảng BarcodeScans KHÔNG tồn tại' 
    END AS KiemTraBangBarcodeScans;

-- Kiểm tra cấu trúc của bảng SanPhams
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'SanPhams' 
AND COLUMN_NAME IN ('Id_SanPham', 'Barcode');

-- Kiểm tra cấu trúc của bảng AspNetUsers
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'AspNetUsers' 
AND COLUMN_NAME = 'Id';

-- Kiểm tra cấu trúc của bảng BarcodeScans
SELECT COLUMN_NAME, DATA_TYPE, CHARACTER_MAXIMUM_LENGTH, IS_NULLABLE 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'BarcodeScans';

-- Thử tạo foreign key đơn giản
ALTER TABLE [dbo].[BarcodeScans] DROP CONSTRAINT IF EXISTS [FK_Test];
GO

BEGIN TRY
    ALTER TABLE [dbo].[BarcodeScans] WITH CHECK ADD 
    CONSTRAINT [FK_Test] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham]);
    
    PRINT 'Foreign key FK_Test tạo thành công';
END TRY
BEGIN CATCH
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH; 