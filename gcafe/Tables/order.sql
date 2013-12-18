CREATE TABLE [dbo].[order]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [branch_id] INT NOT NULL, 
    [table_no] NVARCHAR(10) NOT NULL, 
	[customer_number] INT NOT NULL DEFAULT 1, 
    [open_table_staff_id] INT NOT NULL, 
    [table_opened_time] DATETIME NOT NULL, 
    [receivable] DECIMAL(8, 2) NULL , 
	[net_receipts] DECIMAL(8, 2) NULL, 
    [discount] DECIMAL(4, 2) NULL , 
    [discount_staff_id] INT NULL, 
	[check_out_staff_id] INT NULL, 
    [check_out_time] DATETIME NULL, 
    [member_id] INT NULL, 
    [shift_id] INT NULL, 
    [memo] NTEXT NULL, 
    CONSTRAINT [FK_order_branch] FOREIGN KEY ([branch_id]) REFERENCES [branch]([id]), 
    CONSTRAINT [FK_order_to_staff] FOREIGN KEY ([open_table_staff_id]) REFERENCES [staff]([id]), 
    CONSTRAINT [FK_order_co_staff] FOREIGN KEY ([check_out_staff_id]) REFERENCES [staff]([id]), 
    CONSTRAINT [FK_order_dc_staff] FOREIGN KEY ([discount_staff_id]) REFERENCES [staff]([id]), 
    CONSTRAINT [FK_order_member] FOREIGN KEY ([member_id]) REFERENCES [member]([id]), 
    CONSTRAINT [FK_order_shift] FOREIGN KEY ([shift_id]) REFERENCES [shift]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'开台服务员编号',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order',
    @level2type = N'COLUMN',
    @level2name = 'open_table_staff_id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'开台时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order',
    @level2type = N'COLUMN',
    @level2name = N'table_opened_time'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'打折的服务员id',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order',
    @level2type = N'COLUMN',
    @level2name = N'discount_staff_id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'对应的交班记录',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order',
    @level2type = N'COLUMN',
    @level2name = N'shift_id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'应收款，考虑走单，或买单不够钱的情况',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order',
    @level2type = N'COLUMN',
    @level2name = N'receivable'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'实收款，考虑走单，或买单不够钱的情况',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order',
    @level2type = N'COLUMN',
    @level2name = N'net_receipts'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'记录走单情况',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'order',
    @level2type = N'COLUMN',
    @level2name = N'memo'