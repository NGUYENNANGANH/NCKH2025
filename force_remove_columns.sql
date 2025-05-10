USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Đang xóa cột bằng cách tạo lại bảng...';
    
    -- 1. Tạo bảng tạm cho SanPham_KhuyenMais
    CREATE TABLE #TempSanPhamKhuyenMai (
        Id_SanPham NVARCHAR(450) NOT NULL,
        Id_KhuyenMai NVARCHAR(450) NOT NULL,
        MucGiamGiaRieng FLOAT,
        NgayApDung DATETIME,
        NgayKetThuc DATETIME,
        NgayTao DATETIME,
        NgayCapNhat DATETIME,
        TrangThai BIT,
        SoLuongGioiHan INT,
        ThuTuUuTien INT,
        GhiChu NVARCHAR(MAX)
    );
    
    -- 2. Sao chép dữ liệu của SanPham_KhuyenMais mà không có Id_NguoiTao và Id_NguoiCapNhat
    INSERT INTO #TempSanPhamKhuyenMai (
        Id_SanPham, Id_KhuyenMai, MucGiamGiaRieng, NgayApDung, NgayKetThuc,
        NgayTao, NgayCapNhat, TrangThai, SoLuongGioiHan, ThuTuUuTien, GhiChu
    )
    SELECT
        Id_SanPham, Id_KhuyenMai, MucGiamGiaRieng, NgayApDung, NgayKetThuc,
        NgayTao, NgayCapNhat, TrangThai, SoLuongGioiHan, ThuTuUuTien, GhiChu
    FROM SanPham_KhuyenMais;
    
    -- 3. Xóa các khóa ngoại liên quan đến SanPham_KhuyenMais
    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_SanPham_KhuyenMais_TO_KhuyenMais_Id_KhuyenMai')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP CONSTRAINT [FK_SanPham_KhuyenMais_TO_KhuyenMais_Id_KhuyenMai];
        PRINT 'Đã xóa khóa ngoại FK_SanPham_KhuyenMais_TO_KhuyenMais_Id_KhuyenMai';
    END
    
    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_SanPham_KhuyenMais_TO_SanPhams_Id_SanPham')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] DROP CONSTRAINT [FK_SanPham_KhuyenMais_TO_SanPhams_Id_SanPham];
        PRINT 'Đã xóa khóa ngoại FK_SanPham_KhuyenMais_TO_SanPhams_Id_SanPham';
    END
    
    -- 4. Xóa bảng SanPham_KhuyenMais cũ
    DROP TABLE [dbo].[SanPham_KhuyenMais];
    PRINT 'Đã xóa bảng SanPham_KhuyenMais cũ';
    
    -- 5. Tạo lại bảng SanPham_KhuyenMais không có các cột không sử dụng
    CREATE TABLE [dbo].[SanPham_KhuyenMais] (
        Id_SanPham NVARCHAR(450) NOT NULL,
        Id_KhuyenMai NVARCHAR(450) NOT NULL,
        MucGiamGiaRieng FLOAT,
        NgayApDung DATETIME,
        NgayKetThuc DATETIME,
        NgayTao DATETIME,
        NgayCapNhat DATETIME,
        TrangThai BIT,
        SoLuongGioiHan INT,
        ThuTuUuTien INT,
        GhiChu NVARCHAR(MAX),
        CONSTRAINT [PK_SanPham_KhuyenMais] PRIMARY KEY (Id_SanPham, Id_KhuyenMai)
    );
    PRINT 'Đã tạo lại bảng SanPham_KhuyenMais mới';
    
    -- 6. Khôi phục dữ liệu từ bảng tạm
    INSERT INTO [dbo].[SanPham_KhuyenMais] (
        Id_SanPham, Id_KhuyenMai, MucGiamGiaRieng, NgayApDung, NgayKetThuc,
        NgayTao, NgayCapNhat, TrangThai, SoLuongGioiHan, ThuTuUuTien, GhiChu
    )
    SELECT
        Id_SanPham, Id_KhuyenMai, MucGiamGiaRieng, NgayApDung, NgayKetThuc,
        NgayTao, NgayCapNhat, TrangThai, SoLuongGioiHan, ThuTuUuTien, GhiChu
    FROM #TempSanPhamKhuyenMai;
    PRINT 'Đã khôi phục dữ liệu vào bảng SanPham_KhuyenMais mới';
    
    -- 7. Xóa bảng tạm
    DROP TABLE #TempSanPhamKhuyenMai;
    
    -- 8. Tạo lại các khóa ngoại cho SanPham_KhuyenMais
    ALTER TABLE [dbo].[SanPham_KhuyenMais] WITH CHECK ADD
    CONSTRAINT [FK_SanPham_KhuyenMais_TO_KhuyenMais_Id_KhuyenMai] FOREIGN KEY([Id_KhuyenMai])
    REFERENCES [dbo].[KhuyenMais] ([Id_KhuyenMai])
    ON DELETE CASCADE;
    PRINT 'Đã tạo lại khóa ngoại FK_SanPham_KhuyenMais_TO_KhuyenMais_Id_KhuyenMai';
    
    ALTER TABLE [dbo].[SanPham_KhuyenMais] WITH CHECK ADD
    CONSTRAINT [FK_SanPham_KhuyenMais_TO_SanPhams_Id_SanPham] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham])
    ON DELETE CASCADE;
    PRINT 'Đã tạo lại khóa ngoại FK_SanPham_KhuyenMais_TO_SanPhams_Id_SanPham';
    
    -- 9. Làm tương tự cho bảng Vouchers
    -- Tạo bảng tạm cho Vouchers
    CREATE TABLE #TempVouchers (
        Id_Voucher NVARCHAR(450) NOT NULL,
        MaVoucher NVARCHAR(MAX),
        TenVoucher NVARCHAR(MAX),
        LoaiVoucher NVARCHAR(MAX),
        GiaTri FLOAT,
        GiaTriToiDa FLOAT,
        GiaTriDonHangToiThieu FLOAT,
        NgayBatDau DATETIME,
        NgayKetThuc DATETIME,
        SoLuong INT,
        SoLuongDaSuDung INT,
        TrangThai BIT,
        ApDungChoTatCaSanPham BIT,
        ApDungChoDonHangDauTien BIT,
        MoTa NVARCHAR(MAX),
        NgayTao DATETIME,
        NgayCapNhat DATETIME
    );
    
    -- Sao chép dữ liệu của Vouchers mà không có Id_NguoiTao
    INSERT INTO #TempVouchers (
        Id_Voucher, MaVoucher, TenVoucher, LoaiVoucher, GiaTri, GiaTriToiDa, GiaTriDonHangToiThieu,
        NgayBatDau, NgayKetThuc, SoLuong, SoLuongDaSuDung, TrangThai, ApDungChoTatCaSanPham,
        ApDungChoDonHangDauTien, MoTa, NgayTao, NgayCapNhat
    )
    SELECT
        Id_Voucher, MaVoucher, TenVoucher, LoaiVoucher, GiaTri, GiaTriToiDa, GiaTriDonHangToiThieu,
        NgayBatDau, NgayKetThuc, SoLuong, SoLuongDaSuDung, TrangThai, ApDungChoTatCaSanPham,
        ApDungChoDonHangDauTien, MoTa, NgayTao, NgayCapNhat
    FROM Vouchers;
    
    -- Xóa các khóa ngoại liên quan đến Vouchers
    -- VoucherDanhMucs -> Vouchers
    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_VoucherDanhMucs_TO_Vouchers_Id_Voucher')
    BEGIN
        ALTER TABLE [dbo].[VoucherDanhMucs] DROP CONSTRAINT [FK_VoucherDanhMucs_TO_Vouchers_Id_Voucher];
        PRINT 'Đã xóa khóa ngoại FK_VoucherDanhMucs_TO_Vouchers_Id_Voucher';
    END
    
    -- VoucherSanPhams -> Vouchers
    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_VoucherSanPhams_TO_Vouchers_Id_Voucher')
    BEGIN
        ALTER TABLE [dbo].[VoucherSanPhams] DROP CONSTRAINT [FK_VoucherSanPhams_TO_Vouchers_Id_Voucher];
        PRINT 'Đã xóa khóa ngoại FK_VoucherSanPhams_TO_Vouchers_Id_Voucher';
    END
    
    -- VoucherSuDungs -> Vouchers
    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_VoucherSuDungs_TO_Vouchers_Id_Voucher')
    BEGIN
        ALTER TABLE [dbo].[VoucherSuDungs] DROP CONSTRAINT [FK_VoucherSuDungs_TO_Vouchers_Id_Voucher];
        PRINT 'Đã xóa khóa ngoại FK_VoucherSuDungs_TO_Vouchers_Id_Voucher';
    END
    
    IF EXISTS (SELECT * FROM sys.objects WHERE type = 'F' AND name = 'FK_VoucherSuDungs_Vouchers')
    BEGIN
        ALTER TABLE [dbo].[VoucherSuDungs] DROP CONSTRAINT [FK_VoucherSuDungs_Vouchers];
        PRINT 'Đã xóa khóa ngoại FK_VoucherSuDungs_Vouchers';
    END
    
    -- Xóa bảng Vouchers cũ
    DROP TABLE [dbo].[Vouchers];
    PRINT 'Đã xóa bảng Vouchers cũ';
    
    -- Tạo lại bảng Vouchers không có cột Id_NguoiTao
    CREATE TABLE [dbo].[Vouchers] (
        Id_Voucher NVARCHAR(450) NOT NULL,
        MaVoucher NVARCHAR(MAX),
        TenVoucher NVARCHAR(MAX),
        LoaiVoucher NVARCHAR(MAX),
        GiaTri FLOAT,
        GiaTriToiDa FLOAT,
        GiaTriDonHangToiThieu FLOAT,
        NgayBatDau DATETIME,
        NgayKetThuc DATETIME,
        SoLuong INT,
        SoLuongDaSuDung INT,
        TrangThai BIT,
        ApDungChoTatCaSanPham BIT,
        ApDungChoDonHangDauTien BIT,
        MoTa NVARCHAR(MAX),
        NgayTao DATETIME,
        NgayCapNhat DATETIME,
        CONSTRAINT [PK_Vouchers] PRIMARY KEY (Id_Voucher)
    );
    PRINT 'Đã tạo lại bảng Vouchers mới';
    
    -- Khôi phục dữ liệu từ bảng tạm
    INSERT INTO [dbo].[Vouchers] (
        Id_Voucher, MaVoucher, TenVoucher, LoaiVoucher, GiaTri, GiaTriToiDa, GiaTriDonHangToiThieu,
        NgayBatDau, NgayKetThuc, SoLuong, SoLuongDaSuDung, TrangThai, ApDungChoTatCaSanPham,
        ApDungChoDonHangDauTien, MoTa, NgayTao, NgayCapNhat
    )
    SELECT
        Id_Voucher, MaVoucher, TenVoucher, LoaiVoucher, GiaTri, GiaTriToiDa, GiaTriDonHangToiThieu,
        NgayBatDau, NgayKetThuc, SoLuong, SoLuongDaSuDung, TrangThai, ApDungChoTatCaSanPham,
        ApDungChoDonHangDauTien, MoTa, NgayTao, NgayCapNhat
    FROM #TempVouchers;
    PRINT 'Đã khôi phục dữ liệu vào bảng Vouchers mới';
    
    -- Xóa bảng tạm
    DROP TABLE #TempVouchers;
    
    -- Tạo lại các khóa ngoại cho Vouchers
    -- VoucherDanhMucs -> Vouchers
    ALTER TABLE [dbo].[VoucherDanhMucs] WITH CHECK ADD
    CONSTRAINT [FK_VoucherDanhMucs_TO_Vouchers_Id_Voucher] FOREIGN KEY([Id_Voucher])
    REFERENCES [dbo].[Vouchers] ([Id_Voucher])
    ON DELETE CASCADE;
    PRINT 'Đã tạo lại khóa ngoại FK_VoucherDanhMucs_TO_Vouchers_Id_Voucher';
    
    -- VoucherSanPhams -> Vouchers
    ALTER TABLE [dbo].[VoucherSanPhams] WITH CHECK ADD
    CONSTRAINT [FK_VoucherSanPhams_TO_Vouchers_Id_Voucher] FOREIGN KEY([Id_Voucher])
    REFERENCES [dbo].[Vouchers] ([Id_Voucher])
    ON DELETE CASCADE;
    PRINT 'Đã tạo lại khóa ngoại FK_VoucherSanPhams_TO_Vouchers_Id_Voucher';
    
    -- VoucherSuDungs -> Vouchers
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD
    CONSTRAINT [FK_VoucherSuDungs_Vouchers] FOREIGN KEY([Id_Voucher])
    REFERENCES [dbo].[Vouchers] ([Id_Voucher])
    ON DELETE CASCADE;
    PRINT 'Đã tạo lại khóa ngoại FK_VoucherSuDungs_Vouchers';
    
    COMMIT TRANSACTION;
    PRINT '===============================================================';
    PRINT 'Đã xóa thành công các cột Id_NguoiTao và Id_NguoiCapNhat!';
    PRINT '===============================================================';
    
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