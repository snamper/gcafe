CREATE TABLE [dbo].[order_detail]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[device_id] INT NOT NULL, 
    [order_id] INT NOT NULL, 
    [menu_id] INT NOT NULL, 
	[group_cnt] INT NOT NULL , 
    [quantity] DECIMAL(8, 2) NOT NULL, 
    [price] DECIMAL(8, 2) NOT NULL, 
    [order_staff_id] INT NOT NULL, 
    [order_time] DATETIME NOT NULL, 
    [produced_time] DATETIME NULL, 
    [is_cancle] BIT NOT NULL DEFAULT 0, 
    [cancel_staff_id] INT NULL, 
    [cancel_time] DATE NULL, 
    [memo] TEXT NULL, 
    CONSTRAINT [FK_order_detail_order] FOREIGN KEY ([order_id]) REFERENCES [order]([id]), 
    CONSTRAINT [FK_order_detail_menu] FOREIGN KEY ([menu_id]) REFERENCES [menu]([id]), 
    CONSTRAINT [FK_order_detail_staff] FOREIGN KEY ([order_staff_id]) REFERENCES [staff]([id]), 
    CONSTRAINT [FK_order_detail_device] FOREIGN KEY ([device_id]) REFERENCES [device]([id]),
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
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'出品时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail',
    @level2type = N'COLUMN',
    @level2name = N'produced_time'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'是否被取消',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail',
    @level2type = N'COLUMN',
    @level2name = N'is_cancle'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'那个服务员取消这菜品',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail',
    @level2type = N'COLUMN',
    @level2name = N'cancel_staff_id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'取消的时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail',
    @level2type = N'COLUMN',
    @level2name = N'cancel_time'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'第几次点菜，每次可以点几个品种',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order_detail',
    @level2type = N'COLUMN',
    @level2name = N'group_cnt'