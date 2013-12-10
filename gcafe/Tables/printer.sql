CREATE TABLE [dbo].[printer]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [name] NVARCHAR(20) NOT NULL, 
    [printer_group_id] INT NULL, 
    CONSTRAINT [FK_printer_printer_group] FOREIGN KEY ([printer_group_id]) REFERENCES [printer_group]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'打印机名称',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'printer',
    @level2type = N'COLUMN',
    @level2name = N'name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'属于哪个打印组，Null时不属于任何打印组',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'printer',
    @level2type = N'COLUMN',
    @level2name = N'printer_group_id'