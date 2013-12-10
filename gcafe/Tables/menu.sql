CREATE TABLE [dbo].[Menu]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[branch_id] INT NOT NULL,
    [name] NVARCHAR(50) NOT NULL, 
    [number] VARCHAR(10) NOT NULL, 
	[unit] NVARCHAR(6) NOT NULL,
    [price] DECIMAL(8, 2) NOT NULL, 
    [printer_group_id] INT NOT NULL, 
	[is_setmeal] BIT NOT NULL DEFAULT 0, 
    [is_locked] BIT NOT NULL DEFAULT 0, 
    [sold_out] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_Menu_branch] FOREIGN KEY ([branch_id]) REFERENCES [branch]([id]), 
    CONSTRAINT [FK_Menu_printer_group] FOREIGN KEY ([printer_group_id]) REFERENCES [printer_group]([id]), 
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'菜品名称',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Menu',
    @level2type = N'COLUMN',
    @level2name = N'name'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'菜品编号，按此编号生成二维码',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Menu',
    @level2type = N'COLUMN',
    @level2name = N'number'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'厨房打印组id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Menu',
    @level2type = N'COLUMN',
    @level2name = N'printer_group_id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'该菜品是否被锁住',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Menu',
    @level2type = N'COLUMN',
    @level2name = N'is_locked'
GO

EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'菜品单位，一份或一斤等',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Menu',
    @level2type = N'COLUMN',
    @level2name = N'unit'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否售完',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Menu',
    @level2type = N'COLUMN',
    @level2name = N'sold_out'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'这品种是否套餐',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'Menu',
    @level2type = N'COLUMN',
    @level2name = N'is_setmeal'