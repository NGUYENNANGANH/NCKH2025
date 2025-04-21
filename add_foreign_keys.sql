USE [Ban_Hang]
GO

BEGIN TRY
    BEGIN TRANSACTION;
    
    -- 1. SanPham -> DanhMuc
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_SanPhams_DanhMucs]'))
    BEGIN
        ALTER TABLE [dbo].[SanPhams] WITH CHECK ADD 
        CONSTRAINT [FK_SanPhams_DanhMucs] FOREIGN KEY([Id_DanhMuc])
        REFERENCES [dbo].[DanhMucs] ([Id_DanhMuc]);
        
        PRINT 'Added FK between SanPhams and DanhMucs';
    END
    ELSE
        PRINT 'FK between SanPhams and DanhMucs already exists';

    -- 2. VoucherDanhMuc -> DanhMuc
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VoucherDanhMucs_DanhMucs]'))
    BEGIN
        ALTER TABLE [dbo].[VoucherDanhMucs] WITH CHECK ADD 
        CONSTRAINT [FK_VoucherDanhMucs_DanhMucs] FOREIGN KEY([Id_DanhMuc])
        REFERENCES [dbo].[DanhMucs] ([Id_DanhMuc]);
        
        PRINT 'Added FK between VoucherDanhMucs and DanhMucs';
    END
    ELSE
        PRINT 'FK between VoucherDanhMucs and DanhMucs already exists';

    -- 3. VoucherDanhMuc -> Voucher
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_VoucherDanhMucs_Vouchers]'))
    BEGIN
        ALTER TABLE [dbo].[VoucherDanhMucs] WITH CHECK ADD 
        CONSTRAINT [FK_VoucherDanhMucs_Vouchers] FOREIGN KEY([Id_Voucher])
        REFERENCES [dbo].[Vouchers] ([Id_Voucher]);
        
        PRINT 'Added FK between VoucherDanhMucs and Vouchers';
    END
    ELSE
        PRINT 'FK between VoucherDanhMucs and Vouchers already exists';

    -- 4. Payment -> DonHang
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Payments_DonHangs]'))
    BEGIN
        ALTER TABLE [dbo].[Payments] WITH CHECK ADD 
        CONSTRAINT [FK_Payments_DonHangs] FOREIGN KEY([Id_DonHang])
        REFERENCES [dbo].[DonHangs] ([Id_DonHang]);
        
        PRINT 'Added FK between Payments and DonHangs';
    END
    ELSE
        PRINT 'FK between Payments and DonHangs already exists';

    -- 5. Payment -> AspNetUsers
    IF NOT EXISTS (SELECT * FROM sys.foreign_keys WHERE object_id = OBJECT_ID(N'[dbo].[FK_Payments_AspNetUsers]'))
    BEGIN
        ALTER TABLE [dbo].[Payments] WITH CHECK ADD 
        CONSTRAINT [FK_Payments_AspNetUsers] FOREIGN KEY([Id_User])
        REFERENCES [dbo].[AspNetUsers] ([Id]);
        
        PRINT 'Added FK between Payments and AspNetUsers';
    END
    ELSE
        PRINT 'FK between Payments and AspNetUsers already exists';

    COMMIT TRANSACTION;
    PRINT 'All foreign keys added successfully!';
END TRY
BEGIN CATCH
    ROLLBACK TRANSACTION;
    PRINT 'Error: ' + ERROR_MESSAGE();
END CATCH 