CREATE TABLE [dbo].[sys_info]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [branch_id] INT NOT NULL, 
    [order_cnt] INT NOT NULL DEFAULT 0, 
    [is_festival] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_sys_info_branch] FOREIGN KEY ([branch_id]) REFERENCES [branch]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否用节日价',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'sys_info',
    @level2type = N'COLUMN',
    @level2name = N'is_festival'