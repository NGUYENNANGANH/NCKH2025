@echo off
echo Removing TrangThai column from SanPhams table...
sqlcmd -S localhost -d Ban_Hang -Q "IF EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[SanPhams]') AND name = 'TrangThai') BEGIN ALTER TABLE [dbo].[SanPhams] DROP COLUMN [TrangThai] SELECT 'Column removed successfully.' END ELSE BEGIN SELECT 'Column does not exist.' END"
echo Done.
pause 