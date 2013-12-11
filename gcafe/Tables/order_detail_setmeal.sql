CREATE TABLE [dbo].[order_detail_setmeal]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [order_detail_id] INT NOT NULL, 
    [menu_id] INT NOT NULL, 
    CONSTRAINT [FK_order_detail_setmeal_order_detail] FOREIGN KEY ([order_detail_id]) REFERENCES [order_detail]([id]), 
    CONSTRAINT [FK_order_detail_setmeal_menu] FOREIGN KEY ([menu_id]) REFERENCES [menu]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'只有order_detail.menu_id是套餐才会在这表增加记录',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail_setmeal',
    @level2type = N'COLUMN',
    @level2name = N'order_detail_id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对应套餐项(order_detail.memu_id)内的套餐内容',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail_setmeal',
    @level2type = N'COLUMN',
    @level2name = N'menu_id'