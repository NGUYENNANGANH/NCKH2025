USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- 1. Kiểm tra và đảm bảo kết nối VoucherDanhMuc -> DanhMuc
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
                  WHERE name = 'FK_VoucherDanhMucs_DanhMucs')
    BEGIN
        -- Xóa constraint cũ nếu tồn tại
        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherDanhMucs_DanhMucs')
        BEGIN
            ALTER TABLE [dbo].[VoucherDanhMucs] DROP CONSTRAINT [FK_VoucherDanhMucs_DanhMucs];
        END
        
        -- Tạo constraint mới
        ALTER TABLE [dbo].[VoucherDanhMucs] WITH CHECK ADD
        CONSTRAINT [FK_VoucherDanhMucs_DanhMucs] FOREIGN KEY([Id_DanhMuc])
        REFERENCES [dbo].[DanhMucs] ([Id_DanhMuc])
        ON DELETE CASCADE;
        
        PRINT 'Fixed relationship between VoucherDanhMucs and DanhMucs';
    END
    ELSE
        PRINT 'Relationship between VoucherDanhMucs and DanhMucs already exists';
    
    -- 2. Thêm các mối quan hệ khác nếu còn thiếu
    -- Kiểm tra và đảm bảo kết nối DanhMucs -> SanPhams
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
                  WHERE name = 'FK_SanPhams_DanhMucs')
    BEGIN
        -- Xóa constraint cũ nếu tồn tại
        IF EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SanPhams_DanhMucs')
        BEGIN
            ALTER TABLE [dbo].[SanPhams] DROP CONSTRAINT [FK_SanPhams_DanhMucs];
        END
        
        -- Tạo constraint mới
        ALTER TABLE [dbo].[SanPhams] WITH CHECK ADD
        CONSTRAINT [FK_SanPhams_DanhMucs] FOREIGN KEY([Id_DanhMuc])
        REFERENCES [dbo].[DanhMucs] ([Id_DanhMuc]);
        
        PRINT 'Fixed relationship between SanPhams and DanhMucs';
    END
    ELSE
        PRINT 'Relationship between SanPhams and DanhMucs already exists';
    
    -- 1. Ràng buộc còn thiếu: ChiTietDonHang -> DonHang
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_ChiTietDonHangs_DonHangs')
    BEGIN
        ALTER TABLE [dbo].[ChiTietDonHangs] WITH CHECK ADD 
        CONSTRAINT [FK_ChiTietDonHangs_DonHangs] FOREIGN KEY([Id_DonHang])
        REFERENCES [dbo].[DonHangs] ([Id_DonHang])
        ON DELETE CASCADE;
        
        PRINT 'Added FK_ChiTietDonHangs_DonHangs';
    END
    ELSE
        PRINT 'FK_ChiTietDonHangs_DonHangs already exists';
        
    -- 2. Ràng buộc còn thiếu: VoucherSuDung -> DonHang
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSuDungs_DonHangs')
    BEGIN
        ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
        CONSTRAINT [FK_VoucherSuDungs_DonHangs] FOREIGN KEY([Id_DonHang])
        REFERENCES [dbo].[DonHangs] ([Id_DonHang]);
        
        PRINT 'Added FK_VoucherSuDungs_DonHangs';
    END
    ELSE
        PRINT 'FK_VoucherSuDungs_DonHangs already exists';
        
    -- 3. Ràng buộc còn thiếu: VoucherSuDung -> AspNetUsers (User)
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_VoucherSuDungs_AspNetUsers')
    BEGIN
        ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
        CONSTRAINT [FK_VoucherSuDungs_AspNetUsers] FOREIGN KEY([Id_User])
        REFERENCES [dbo].[AspNetUsers] ([Id]);
        
        PRINT 'Added FK_VoucherSuDungs_AspNetUsers';
    END
    ELSE
        PRINT 'FK_VoucherSuDungs_AspNetUsers already exists';
        
    -- 4. Ràng buộc còn thiếu: Voucher -> AspNetUsers (NguoiTao)
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_Vouchers_AspNetUsers')
    BEGIN
        ALTER TABLE [dbo].[Vouchers] WITH CHECK ADD 
        CONSTRAINT [FK_Vouchers_AspNetUsers] FOREIGN KEY([Id_NguoiTao])
        REFERENCES [dbo].[AspNetUsers] ([Id]);
        
        PRINT 'Added FK_Vouchers_AspNetUsers';
    END
    ELSE
        PRINT 'FK_Vouchers_AspNetUsers already exists';
        
    -- 5. Ràng buộc còn thiếu: SanPham_KhuyenMai -> AspNetUsers (NguoiTao)
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SanPham_KhuyenMais_AspNetUsers_NguoiTao')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] WITH CHECK ADD 
        CONSTRAINT [FK_SanPham_KhuyenMais_AspNetUsers_NguoiTao] FOREIGN KEY([Id_NguoiTao])
        REFERENCES [dbo].[AspNetUsers] ([Id]);
        
        PRINT 'Added FK_SanPham_KhuyenMais_AspNetUsers_NguoiTao';
    END
    ELSE
        PRINT 'FK_SanPham_KhuyenMais_AspNetUsers_NguoiTao already exists';
        
    -- 6. Ràng buộc còn thiếu: SanPham_KhuyenMai -> AspNetUsers (NguoiCapNhat)
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE name = 'FK_SanPham_KhuyenMais_AspNetUsers_NguoiCapNhat')
    BEGIN
        ALTER TABLE [dbo].[SanPham_KhuyenMais] WITH CHECK ADD 
        CONSTRAINT [FK_SanPham_KhuyenMais_AspNetUsers_NguoiCapNhat] FOREIGN KEY([Id_NguoiCapNhat])
        REFERENCES [dbo].[AspNetUsers] ([Id]);
        
        PRINT 'Added FK_SanPham_KhuyenMais_AspNetUsers_NguoiCapNhat';
    END
    ELSE
        PRINT 'FK_SanPham_KhuyenMais_AspNetUsers_NguoiCapNhat already exists';
    
    COMMIT TRANSACTION;
    PRINT 'All relationships have been fixed successfully!';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH 