USE [Ban_Hang]
GO

-- Script để tạo lại diagram trong SQL Server Management Studio
-- Lưu ý: Cần phải chạy script này trong SSMS và sau đó làm mới các diagram

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Xóa diagrams hiện có nếu có
    IF EXISTS (SELECT * FROM sys.extended_properties WHERE name = 'microsoft_database_tools_support')
    BEGIN
        -- Xóa các diagrams hiện có
        EXEC sp_executesql N'
        DELETE FROM dbo.sysdiagrams WHERE name LIKE ''%''
        ';
        PRINT 'Đã xóa tất cả các diagrams hiện có';
    END
    
    -- Thông báo cần làm mới SSMS để thấy thay đổi
    PRINT 'Hãy làm mới (refresh) SQL Server Management Studio và tạo lại diagram';
    PRINT 'Các khóa ngoại đã được cập nhật chính xác trong cơ sở dữ liệu';
    PRINT 'Các mối quan hệ từ bảng VoucherSuDungs đến các bảng:';
    PRINT '1. FK_VoucherSuDungs_Vouchers: VoucherSuDungs.Id_Voucher -> Vouchers.Id_Voucher';
    PRINT '2. FK_VoucherSuDungs_DonHangs: VoucherSuDungs.Id_DonHang -> DonHangs.Id_DonHang';
    PRINT '3. FK_VoucherSuDungs_AspNetUsers: VoucherSuDungs.Id_User -> AspNetUsers.Id';
    
    -- Gợi ý để tạo mới diagram
    PRINT '------------------------------------------------------------------------';
    PRINT 'HƯỚNG DẪN TẠO LẠI DIAGRAM:';
    PRINT '1. Mở SQL Server Management Studio';
    PRINT '2. Kết nối đến server và mở cơ sở dữ liệu Ban_Hang';
    PRINT '3. Mở thư mục Database Diagrams';
    PRINT '4. Nhấp chuột phải và chọn "New Database Diagram"';
    PRINT '5. Thêm các bảng cần thiết vào diagram';
    PRINT '6. Lưu lại diagram với tên mới';
    PRINT '------------------------------------------------------------------------';
    
    COMMIT TRANSACTION;
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Lỗi: ' + ERROR_MESSAGE();
END CATCH 