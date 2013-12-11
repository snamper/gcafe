CREATE TABLE [dbo].[order_detail_method]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [order_detail_id] INT NULL, 
    [order_detail_setmeal_id] INT NULL, 
    [method_id] INT NOT NULL, 
    [price] DECIMAL(8, 2) NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_order_detail_method_order_detail] FOREIGN KEY ([order_detail_id]) REFERENCES [order_detail]([id]), 
    CONSTRAINT [FK_order_detail_method_order_detail_setmeal] FOREIGN KEY ([order_detail_setmeal_id]) REFERENCES [order_detail_setmeal]([id]), 
    CONSTRAINT [FK_order_detail_method_method] FOREIGN KEY ([method_id]) REFERENCES [method]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'这记录有可能对应order_detail或order_detail_setmeal',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail_method',
    @level2type = N'COLUMN',
    @level2name = N'order_detail_id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'这记录有可能对应order_detail或order_detail_setmeal',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail_method',
    @level2type = N'COLUMN',
    @level2name = N'order_detail_setmeal_id'