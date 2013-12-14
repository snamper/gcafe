CREATE TABLE [dbo].[shift]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [sum] DECIMAL(10, 2) NOT NULL, 
    [time] DATETIME NOT NULL, 
    [staff_id] NCHAR(10) NULL
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交班时的金额',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'shift',
    @level2type = N'COLUMN',
    @level2name = N'sum'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交班操作时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'shift',
    @level2type = N'COLUMN',
    @level2name = N'time'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'交班操作的员工',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'shift',
    @level2type = N'COLUMN',
    @level2name = N'staff_id'