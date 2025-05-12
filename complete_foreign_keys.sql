USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    PRINT 'Starting comprehensive foreign key relationship setup...';
    
    -- First, drop all existing non-identity foreign keys to avoid conflicts
    DECLARE @DropConstraintSQL NVARCHAR(MAX) = '';
    
    SELECT @DropConstraintSQL = @DropConstraintSQL + 
        'ALTER TABLE [' + OBJECT_SCHEMA_NAME(parent_object_id) + '].[' + OBJECT_NAME(parent_object_id) + '] ' +
        'DROP CONSTRAINT [' + name + '];' + CHAR(13) + CHAR(10)
    FROM sys.foreign_keys
    WHERE name NOT LIKE 'FK_AspNet%'
    ORDER BY name;
    
    PRINT 'Removing all existing non-identity foreign keys...';
    EXEC sp_executesql @DropConstraintSQL;
    
    -- Now create all foreign key relationships with consistent naming and proper cascading behavior
    PRINT 'Creating all foreign key relationships with consistent naming...';
    
    -- 1. SanPham -> DanhMuc (non-cascading as a product can exist even if category is deleted)
    ALTER TABLE [dbo].[SanPhams] WITH CHECK ADD 
    CONSTRAINT [FK_SanPhams_DanhMucs] FOREIGN KEY([Id_DanhMuc])
    REFERENCES [dbo].[DanhMucs] ([Id_DanhMuc]);
    PRINT 'Added FK_SanPhams_DanhMucs';
    
    -- 2. ChiTietDonHang -> DonHang (cascading delete as order details depend on order)
    ALTER TABLE [dbo].[ChiTietDonHangs] WITH CHECK ADD 
    CONSTRAINT [FK_ChiTietDonHangs_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang])
    ON DELETE CASCADE;
    PRINT 'Added FK_ChiTietDonHangs_DonHangs';
    
    -- 3. ChiTietDonHang -> SanPham (non-cascading as we want to preserve order history)
    ALTER TABLE [dbo].[ChiTietDonHangs] WITH CHECK ADD 
    CONSTRAINT [FK_ChiTietDonHangs_SanPhams] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham]);
    PRINT 'Added FK_ChiTietDonHangs_SanPhams';
    
    -- 4. ChiTietGioHang -> GioHang (cascading as cart items depend on cart)
    ALTER TABLE [dbo].[ChiTietGioHangs] WITH CHECK ADD 
    CONSTRAINT [FK_ChiTietGioHangs_GioHangs] FOREIGN KEY([Id_GioHang])
    REFERENCES [dbo].[GioHangs] ([Id_GioHang])
    ON DELETE CASCADE;
    PRINT 'Added FK_ChiTietGioHangs_GioHangs';
    
    -- 5. ChiTietGioHang -> SanPham (non-cascading as cart can exist with removed products)
    ALTER TABLE [dbo].[ChiTietGioHangs] WITH CHECK ADD 
    CONSTRAINT [FK_ChiTietGioHangs_SanPhams] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham]);
    PRINT 'Added FK_ChiTietGioHangs_SanPhams';
    
    -- 6. DanhGia -> User (non-cascading as reviews should persist)
    ALTER TABLE [dbo].[DanhGia] WITH CHECK ADD 
    CONSTRAINT [FK_DanhGia_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_DanhGia_AspNetUsers';
    
    -- 7. DanhGia -> SanPham (cascading as reviews should be deleted if product is deleted)
    ALTER TABLE [dbo].[DanhGia] WITH CHECK ADD 
    CONSTRAINT [FK_DanhGia_SanPhams] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham])
    ON DELETE CASCADE;
    PRINT 'Added FK_DanhGia_SanPhams';
    
    -- 8. DonHang -> User (Check if column is renamed to User_Id based on earlier scripts)
    IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.DonHangs') AND name = 'User_Id')
    BEGIN
        ALTER TABLE [dbo].[DonHangs] WITH CHECK ADD 
        CONSTRAINT [FK_DonHangs_AspNetUsers] FOREIGN KEY([User_Id])
        REFERENCES [dbo].[AspNetUsers] ([Id]);
        PRINT 'Added FK_DonHangs_AspNetUsers (referencing User_Id)';
    END
    ELSE IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('dbo.DonHangs') AND name = 'Id_KH')
    BEGIN
        ALTER TABLE [dbo].[DonHangs] WITH CHECK ADD 
        CONSTRAINT [FK_DonHangs_AspNetUsers] FOREIGN KEY([Id_KH])
        REFERENCES [dbo].[AspNetUsers] ([Id]);
        PRINT 'Added FK_DonHangs_AspNetUsers (referencing Id_KH)';
    END
    
    -- 9. GioHang -> User
    ALTER TABLE [dbo].[GioHangs] WITH CHECK ADD 
    CONSTRAINT [FK_GioHangs_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_GioHangs_AspNetUsers';
    
    -- 10. HoaDon -> User (non-cascading as invoices should persist)
    ALTER TABLE [dbo].[HoaDons] WITH CHECK ADD 
    CONSTRAINT [FK_HoaDons_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_HoaDons_AspNetUsers';
    
    -- 11. HoaDon -> DonHang (cascading as invoice depends on order)
    ALTER TABLE [dbo].[HoaDons] WITH CHECK ADD 
    CONSTRAINT [FK_HoaDons_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang])
    ON DELETE CASCADE;
    PRINT 'Added FK_HoaDons_DonHangs';
    
    -- 12. Payment -> DonHang (cascading as payment depends on order)
    ALTER TABLE [dbo].[Payments] WITH CHECK ADD 
    CONSTRAINT [FK_Payments_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang])
    ON DELETE CASCADE;
    PRINT 'Added FK_Payments_DonHangs';
    
    -- 13. Payment -> User (non-cascading as payment records should persist)
    ALTER TABLE [dbo].[Payments] WITH CHECK ADD 
    CONSTRAINT [FK_Payments_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_Payments_AspNetUsers';
    
    -- 14. SanPham_KhuyenMai -> KhuyenMai (cascading as promotion items depend on promotion)
    ALTER TABLE [dbo].[SanPham_KhuyenMais] WITH CHECK ADD 
    CONSTRAINT [FK_SanPham_KhuyenMais_KhuyenMais] FOREIGN KEY([Id_KhuyenMai])
    REFERENCES [dbo].[KhuyenMais] ([Id_KhuyenMai])
    ON DELETE CASCADE;
    PRINT 'Added FK_SanPham_KhuyenMais_KhuyenMais';
    
    -- 15. SanPham_KhuyenMai -> SanPham (cascading as promotion items depend on product)
    ALTER TABLE [dbo].[SanPham_KhuyenMais] WITH CHECK ADD 
    CONSTRAINT [FK_SanPham_KhuyenMais_SanPhams] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham])
    ON DELETE CASCADE;
    PRINT 'Added FK_SanPham_KhuyenMais_SanPhams';
    
    -- 16. VoucherDanhMuc -> DanhMuc (cascading as voucher categories depend on category)
    ALTER TABLE [dbo].[VoucherDanhMucs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherDanhMucs_DanhMucs] FOREIGN KEY([Id_DanhMuc])
    REFERENCES [dbo].[DanhMucs] ([Id_DanhMuc])
    ON DELETE CASCADE;
    PRINT 'Added FK_VoucherDanhMucs_DanhMucs';
    
    -- 17. VoucherDanhMuc -> Voucher (cascading as voucher categories depend on voucher)
    ALTER TABLE [dbo].[VoucherDanhMucs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherDanhMucs_Vouchers] FOREIGN KEY([Id_Voucher])
    REFERENCES [dbo].[Vouchers] ([Id_Voucher])
    ON DELETE CASCADE;
    PRINT 'Added FK_VoucherDanhMucs_Vouchers';
    
    -- 18. VoucherSanPham -> SanPham (cascading as voucher products depend on product)
    ALTER TABLE [dbo].[VoucherSanPhams] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSanPhams_SanPhams] FOREIGN KEY([Id_SanPham])
    REFERENCES [dbo].[SanPhams] ([Id_SanPham])
    ON DELETE CASCADE;
    PRINT 'Added FK_VoucherSanPhams_SanPhams';
    
    -- 19. VoucherSanPham -> Voucher (cascading as voucher products depend on voucher)
    ALTER TABLE [dbo].[VoucherSanPhams] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSanPhams_Vouchers] FOREIGN KEY([Id_Voucher])
    REFERENCES [dbo].[Vouchers] ([Id_Voucher])
    ON DELETE CASCADE;
    PRINT 'Added FK_VoucherSanPhams_Vouchers';
    
    -- 20. VoucherSuDung -> Voucher (cascading as voucher usage depends on voucher)
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_Vouchers] FOREIGN KEY([Id_Voucher])
    REFERENCES [dbo].[Vouchers] ([Id_Voucher])
    ON DELETE CASCADE;
    PRINT 'Added FK_VoucherSuDungs_Vouchers';
    
    -- 21. VoucherSuDung -> DonHang (cascading as voucher usage depends on order)
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_DonHangs] FOREIGN KEY([Id_DonHang])
    REFERENCES [dbo].[DonHangs] ([Id_DonHang])
    ON DELETE CASCADE;
    PRINT 'Added FK_VoucherSuDungs_DonHangs';
    
    -- 22. VoucherSuDung -> User (non-cascading as voucher usage records should persist)
    ALTER TABLE [dbo].[VoucherSuDungs] WITH CHECK ADD 
    CONSTRAINT [FK_VoucherSuDungs_AspNetUsers] FOREIGN KEY([Id_User])
    REFERENCES [dbo].[AspNetUsers] ([Id]);
    PRINT 'Added FK_VoucherSuDungs_AspNetUsers';
    
    COMMIT TRANSACTION;
    PRINT '===============================================================';
    PRINT 'All foreign key relationships have been successfully established!';
    PRINT '===============================================================';
    PRINT 'Key improvements:';
    PRINT '1. Consistent naming convention for all foreign keys';
    PRINT '2. Proper cascading delete applied where appropriate';
    PRINT '3. Removal of invalid references to removed columns';
    PRINT '4. Support for both Id_KH and User_Id column names in DonHangs table';
    PRINT '===============================================================';
    PRINT 'Note: Refresh your SQL Server Management Studio to see the changes in diagrams';
END TRY
BEGIN CATCH
    IF @@TRANCOUNT > 0
        ROLLBACK TRANSACTION;
    
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH 