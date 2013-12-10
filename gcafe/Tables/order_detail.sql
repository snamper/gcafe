CREATE TABLE [dbo].[order_detail]
(
	[id] INT NOT NULL PRIMARY KEY, 
    [order_id] INT NOT NULL, 
    [menu_id] INT NOT NULL, 
    [quantity] DECIMAL(8, 2) NOT NULL, 
    [price] DECIMAL(8, 2) NOT NULL, 
    [order_staff_id] INT NOT NULL, 
    [order_time] DATETIME NOT NULL, 
    CONSTRAINT [FK_order_detail_order] FOREIGN KEY ([order_id]) REFERENCES [order]([id]), 
    CONSTRAINT [FK_order_detail_menu] FOREIGN KEY ([menu_id]) REFERENCES [menu]([id]), 
    CONSTRAINT [FK_order_detail_staff] FOREIGN KEY ([order_staff_id]) REFERENCES [staff]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'点这项菜的服务员',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail',
    @level2type = N'COLUMN',
    @level2name = N'order_staff_id'