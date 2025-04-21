-- Script to remove the TrangThai column from SanPhams table
USE [Ban_Hang]
GO

-- Check if the column exists before trying to remove it
IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SanPhams]') AND name = 'TrangThai')
BEGIN
    PRINT 'Removing TrangThai column from SanPhams table...'
    ALTER TABLE [dbo].[SanPhams] DROP COLUMN [TrangThai]
    PRINT 'Column removed successfully.'
END
ELSE
BEGIN
    PRINT 'TrangThai column does not exist in SanPhams table.'
END
GO 