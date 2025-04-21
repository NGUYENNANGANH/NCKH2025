USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- Gỡ bỏ tất cả các ràng buộc không phải Identity Framework
    DECLARE @DropConstraintSQL NVARCHAR(MAX) = '';
    
    SELECT @DropConstraintSQL = @DropConstraintSQL + 
        'ALTER TABLE [' + OBJECT_SCHEMA_NAME(parent_object_id) + '].[' + OBJECT_NAME(parent_object_id) + '] ' +
        'DROP CONSTRAINT [' + name + '];' + CHAR(13) + CHAR(10)
    FROM sys.foreign_keys
    WHERE name NOT LIKE 'FK_AspNet%'
    ORDER BY name;
    
    PRINT 'Removing all current foreign keys...';
    EXEC sp_executesql @DropConstraintSQL;
    
    -- Tạo lại các ràng buộc với quy ước đặt tên nhất quán: FK_Table1_TO_Table2
    
    -- 1. SanPham -> DanhMuc
    ALTER TABLE [dbo].[SanPhams] WITH CHECK ADD 
    CONSTRAINT [FK_SanPhams_TO_DanhMucs] FOREIGN KEY([Id_DanhMuc])
    REFERENCES [dbo].[DanhMucs] ([Id_DanhMuc]);
    PRINT 'Added FK_SanPhams_TO_DanhMucs';
    
    -- 2. ChiTietDonHang -> DonHang
    ALTER TABLE [dbo].[ChiTietDonHangs] WITH CHECK ADD 
    CONSTRAINT [FK_ChiTietDonHangs_TO_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang])
    ON DELETE CASCADE;
    PRINT 'Added FK_ChiTietDonHangs_TO_DonHangs';
    
    -- 3. ChiTietDonHang -> SanPham
    ALTER TABLE [dbo].[ChiTietDonHangs] WITH CHECK ADD 
    CONSTRAINT [FK_ChiTietDonHangs_TO_SanPhams] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham]);
    PRINT 'Added FK_ChiTietDonHangs_TO_SanPhams';
    
    -- 4. ChiTietGioHang -> GioHang
    ALTER TABLE [dbo].[ChiTietGioHangs] WITH CHECK ADD 
    CONSTRAINT [FK_ChiTietGioHangs_TO_GioHangs] FOREIGN KEY([Id_GioHang])
    REFERENCES [dbo].[GioHangs] ([Id_GioHang])
    ON DELETE CASCADE;
    PRINT 'Added FK_ChiTietGioHangs_TO_GioHangs';
    
    -- 5. ChiTietGioHang -> SanPham
    ALTER TABLE [dbo].[ChiTietGioHangs] WITH CHECK ADD 
    CONSTRAINT [FK_ChiTietGioHangs_TO_SanPhams] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham]);
    PRINT 'Added FK_ChiTietGioHangs_TO_SanPhams';
    
    -- 6. DanhGia -> User
    ALTER TABLE [dbo].[DanhGia] WITH CHECK ADD 
    CONSTRAINT [FK_DanhGia_TO_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_DanhGia_TO_AspNetUsers';
    
    -- 7. DanhGia -> SanPham
    ALTER TABLE [dbo].[DanhGia] WITH CHECK ADD 
    CONSTRAINT [FK_DanhGia_TO_SanPhams] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham])
    ON DELETE CASCADE;
    PRINT 'Added FK_DanhGia_TO_SanPhams';
    
    -- 8. DonHang -> User
    ALTER TABLE [dbo].[DonHangs] WITH CHECK ADD 
    CONSTRAINT [FK_DonHangs_TO_AspNetUsers] FOREIGN KEY([Id_KH])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_DonHangs_TO_AspNetUsers';
    
    -- 9. GioHang -> User
    ALTER TABLE [dbo].[GioHangs] WITH CHECK ADD 
    CONSTRAINT [FK_GioHangs_TO_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_GioHangs_TO_AspNetUsers';
    
    -- 10. HoaDon -> User
    ALTER TABLE [dbo].[HoaDons] WITH CHECK ADD 
    CONSTRAINT [FK_HoaDons_TO_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_HoaDons_TO_AspNetUsers';
    
    -- 11. HoaDon -> DonHang
    ALTER TABLE [dbo].[HoaDons] WITH CHECK ADD 
    CONSTRAINT [FK_HoaDons_TO_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang]);
    PRINT 'Added FK_HoaDons_TO_DonHangs';
    
    -- 12. Payment -> DonHang
    ALTER TABLE [dbo].[Payments] WITH CHECK ADD 
    CONSTRAINT [FK_Payments_TO_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang]);
    PRINT 'Added FK_Payments_TO_DonHangs';
    
    -- 13. Payment -> User
    ALTER TABLE [dbo].[Payments] WITH CHECK ADD 
    CONSTRAINT [FK_Payments_TO_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_Payments_TO_AspNetUsers';
    
    -- 14. SanPham_KhuyenMai -> KhuyenMai
    ALTER TABLE [dbo].[SanPham_KhuyenMais] WITH CHECK ADD 
    CONSTRAINT [FK_SanPham_KhuyenMais_TO_KhuyenMais] FOREIGN KEY([Id_KhuyenMai])
    REFERENCES [dbo].[KhuyenMais] ([Id_KhuyenMai])
    ON DELETE CASCADE;
    PRINT 'Added FK_SanPham_KhuyenMais_TO_KhuyenMais';
    
    -- 15. SanPham_KhuyenMai -> SanPham
    ALTER TABLE [dbo].[SanPham_KhuyenMais] WITH CHECK ADD 
    CONSTRAINT [FK_SanPham_KhuyenMais_TO_SanPhams] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham])
    ON DELETE CASCADE;
    PRINT 'Added FK_SanPham_KhuyenMais_TO_SanPhams';
    
    -- 16. SanPham_KhuyenMai -> NguoiTao
    ALTER TABLE [dbo].[SanPham_KhuyenMais] WITH CHECK ADD 
    CONSTRAINT [FK_SanPham_KhuyenMais_TO_AspNetUsers_NguoiTao] FOREIGN KEY([Id_NguoiTao])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_SanPham_KhuyenMais_TO_AspNetUsers_NguoiTao';
    
    -- 17. SanPham_KhuyenMai -> NguoiCapNhat
    ALTER TABLE [dbo].[SanPham_KhuyenMais] WITH CHECK ADD 
    CONSTRAINT [FK_SanPham_KhuyenMais_TO_AspNetUsers_NguoiCapNhat] FOREIGN KEY([Id_NguoiCapNhat])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_SanPham_KhuyenMais_TO_AspNetUsers_NguoiCapNhat';
    
    -- 18. Voucher -> NguoiTao
    ALTER TABLE [dbo].[Vouchers] WITH CHECK ADD 
    CONSTRAINT [FK_Vouchers_TO_AspNetUsers] FOREIGN KEY([Id_NguoiTao])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_Vouchers_TO_AspNetUsers';
    
    -- 19. VoucherDanhMuc -> DanhMuc
    ALTER TABLE [dbo].[VoucherDanhMucs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherDanhMucs_TO_DanhMucs] FOREIGN KEY([Id_DanhMuc])
    REFERENCES [dbo].[DanhMucs] ([Id_DanhMuc])
    ON DELETE CASCADE;
    PRINT 'Added FK_VoucherDanhMucs_TO_DanhMucs';
    
    -- 20. VoucherDanhMuc -> Voucher
    ALTER TABLE [dbo].[VoucherDanhMucs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherDanhMucs_TO_Vouchers] FOREIGN KEY([Id_Voucher])
    REFERENCES [dbo].[Vouchers] ([Id_Voucher])
    ON DELETE CASCADE;
    PRINT 'Added FK_VoucherDanhMucs_TO_Vouchers';
    
    -- 21. VoucherSanPham -> SanPham
    ALTER TABLE [dbo].[VoucherSanPhams] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSanPhams_TO_SanPhams] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham])
    ON DELETE CASCADE;
    PRINT 'Added FK_VoucherSanPhams_TO_SanPhams';
    
    -- 22. VoucherSanPham -> Voucher
    ALTER TABLE [dbo].[VoucherSanPhams] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSanPhams_TO_Vouchers] FOREIGN KEY([Id_Voucher])
    REFERENCES [dbo].[Vouchers] ([Id_Voucher])
    ON DELETE CASCADE;
    PRINT 'Added FK_VoucherSanPhams_TO_Vouchers';
    
    -- 23. VoucherSuDung -> Voucher
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_TO_Vouchers] FOREIGN KEY([Id_Voucher])
    REFERENCES [dbo].[Vouchers] ([Id_Voucher])
    ON DELETE CASCADE;
    PRINT 'Added FK_VoucherSuDungs_TO_Vouchers';
    
    -- 24. VoucherSuDung -> DonHang
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_TO_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang]);
    PRINT 'Added FK_VoucherSuDungs_TO_DonHangs';
    
    -- 25. VoucherSuDung -> User
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_TO_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_VoucherSuDungs_TO_AspNetUsers';
    
    COMMIT TRANSACTION;
    PRINT 'All relationships have been standardized successfully!';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH 