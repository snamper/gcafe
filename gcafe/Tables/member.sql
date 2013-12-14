CREATE TABLE [dbo].[member]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[idcard_number] NVARCHAR(50) NOT NULL,
    [name] NVARCHAR(20) NOT NULL, 
    [phone_num] NVARCHAR(20) NULL, 
    [email] NVARCHAR(50) NULL, 
    [point] INT NOT NULL DEFAULT 0, 
    [credit] DECIMAL(10, 2) NOT NULL DEFAULT 0, 
    [join_date] DATE NOT NULL 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'加入会员的时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'member',
    @level2type = N'COLUMN',
    @level2name = N'join_date'