CREATE TABLE [dbo].[order_detail_print]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [order_detail_id] INT NOT NULL, 
    [menu_id] INT NOT NULL, 
    [setmeal_menu_id] INT NULL, 
    [method] NVARCHAR(50) NULL, 
    [serving] NVARCHAR(10) NULL, 
    CONSTRAINT [FK_order_detail_print_order_detail] FOREIGN KEY ([order_detail_id]) REFERENCES [order_detail]([id]), 
    CONSTRAINT [FK_order_detail_print_menu] FOREIGN KEY ([menu_id]) REFERENCES [menu]([id]), 
    CONSTRAINT [FK_order_detail_print_setmeal] FOREIGN KEY ([setmeal_menu_id]) REFERENCES [menu]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'一条记录中，如果setmeal_menu_id不为Null的话，menu_id项为setmeal_menu_id里的项目',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail_print',
    @level2type = N'COLUMN',
    @level2name = 'setmeal_menu_id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'做法，每个做法用逗号分开',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail_print',
    @level2type = N'COLUMN',
    @level2name = N'method'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'上菜方式，如即起，等叫',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail_print',
    @level2type = N'COLUMN',
    @level2name = N'serving'