CREATE TABLE [dbo].[credit]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [order_id] INT NOT NULL, 
    [member_id] INT NULL, 
    [name] NVARCHAR(50) NULL, 
    [phone] NVARCHAR(50) NULL, 
    [repay_time] DATETIME NULL, 
    CONSTRAINT [FK_credit_order] FOREIGN KEY ([order_id]) REFERENCES [order]([id]), 
    CONSTRAINT [FK_credit_member] FOREIGN KEY ([member_id]) REFERENCES [member]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'那笔交易出现赊账',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'credit',
    @level2type = N'COLUMN',
    @level2name = N'order_id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'还款时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'credit',
    @level2type = N'COLUMN',
    @level2name = N'repay_time'