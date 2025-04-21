USE [Ban_Hang]
GO

-- Kiểm tra xem bảng SanPhams có tồn tại không
SELECT 
    CASE 
        WHEN EXISTS (SELECT * FROM sys.tables WHERE name = 'SanPhams') 
        THEN 'Bảng SanPhams tồn tại' 
        ELSE 'Bảng SanPhams KHÔNG tồn tại' 
    END AS [KiemTraBangSanPhams];

-- Kiểm tra xem bảng AspNetUsers có tồn tại không
SELECT 
    CASE 
        WHEN EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers') 
        THEN 'Bảng AspNetUsers tồn tại' 
        ELSE 'Bảng AspNetUsers KHÔNG tồn tại' 
    END AS [KiemTraBangAspNetUsers];

-- Kiểm tra xem bảng BarcodeScans có tồn tại không
SELECT 
    CASE 
        WHEN EXISTS (SELECT * FROM sys.tables WHERE name = 'BarcodeScans') 
        THEN 'Bảng BarcodeScans tồn tại' 
        ELSE 'Bảng BarcodeScans KHÔNG tồn tại' 
    END AS [KiemTraBangBarcodeScans];

-- Kiểm tra xem khóa chính của SanPhams
SELECT 
    COLUMN_NAME AS PrimaryKeyColumn, 
    DATA_TYPE AS DataType,
    CHARACTER_MAXIMUM_LENGTH AS MaxLength
FROM 
    INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
JOIN 
    INFORMATION_SCHEMA.COLUMNS c ON kcu.TABLE_NAME = c.TABLE_NAME AND kcu.COLUMN_NAME = c.COLUMN_NAME
WHERE 
    kcu.TABLE_NAME = 'SanPhams' 
    AND kcu.CONSTRAINT_NAME LIKE 'PK%';

-- Kiểm tra xem khóa chính của AspNetUsers
SELECT 
    COLUMN_NAME AS PrimaryKeyColumn, 
    DATA_TYPE AS DataType,
    CHARACTER_MAXIMUM_LENGTH AS MaxLength
FROM 
    INFORMATION_SCHEMA.KEY_COLUMN_USAGE kcu
JOIN 
    INFORMATION_SCHEMA.COLUMNS c ON kcu.TABLE_NAME = c.TABLE_NAME AND kcu.COLUMN_NAME = c.COLUMN_NAME
WHERE 
    kcu.TABLE_NAME = 'AspNetUsers' 
    AND kcu.CONSTRAINT_NAME LIKE 'PK%';

-- Kiểm tra cấu trúc của bảng BarcodeScans
SELECT 
    COLUMN_NAME AS BarcodeScansColumn, 
    DATA_TYPE AS DataType,
    CHARACTER_MAXIMUM_LENGTH AS MaxLength,
    IS_NULLABLE AS AllowNull
FROM 
    INFORMATION_SCHEMA.COLUMNS 
WHERE 
    TABLE_NAME = 'BarcodeScans';

-- Kiểm tra NULL trong BarcodeScans.Id_SanPham
SELECT 
    COUNT(*) AS TotalRows,
    SUM(CASE WHEN Id_SanPham IS NULL THEN 1 ELSE 0 END) AS NullIdSanPhamCount
FROM 
    BarcodeScans;

-- Kiểm tra NULL trong BarcodeScans.User_Id
SELECT 
    COUNT(*) AS TotalRows,
    SUM(CASE WHEN User_Id IS NULL THEN 1 ELSE 0 END) AS NullUserIdCount
FROM 
    BarcodeScans;

-- Kiểm tra các khóa ngoại đã tồn tại trên BarcodeScans
SELECT 
    f.name AS ForeignKeyName,
    OBJECT_NAME(f.parent_object_id) AS TableName,
    COL_NAME(fc.parent_object_id, fc.parent_column_id) AS ColumnName,
    OBJECT_NAME (f.referenced_object_id) AS ReferenceTableName,
    COL_NAME(fc.referenced_object_id, fc.referenced_column_id) AS ReferenceColumnName
FROM 
    sys.foreign_keys AS f
INNER JOIN 
    sys.foreign_key_columns AS fc ON f.OBJECT_ID = fc.constraint_object_id
WHERE 
    OBJECT_NAME(f.parent_object_id) = 'BarcodeScans';

-- Kiểm tra các chỉ mục đã tồn tại trên BarcodeScans
SELECT 
    i.name AS IndexName,
    COL_NAME(ic.object_id, ic.column_id) AS ColumnName,
    i.type_desc AS IndexType,
    i.is_unique AS IsUnique
FROM 
    sys.indexes AS i
INNER JOIN 
    sys.index_columns AS ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
WHERE 
    OBJECT_NAME(i.object_id) = 'BarcodeScans'
ORDER BY 
    i.name, ic.key_ordinal; 