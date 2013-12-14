CREATE TABLE [dbo].[printer_group]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[branch_id] INT NOT NULL,
    [name] NVARCHAR(50) NOT NULL, 
    CONSTRAINT [FK_printer_group_branch] FOREIGN KEY ([branch_id]) REFERENCES [branch]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'打印组名称',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'printer_group',
    @level2type = N'COLUMN',
    @level2name = N'name'