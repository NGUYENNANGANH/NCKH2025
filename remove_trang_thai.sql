USE [Ban_Hang]
GO

IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SanPhams]') AND name = 'TrangThai')
BEGIN
    ALTER TABLE [dbo].[SanPhams] DROP COLUMN [TrangThai]
    PRINT 'Column TrangThai has been dropped from SanPhams table'
END
ELSE
BEGIN
    PRINT 'Column TrangThai does not exist in SanPhams table'
END
GO 