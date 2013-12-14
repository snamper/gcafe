CREATE TABLE [dbo].[member_refill]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [branch_id] INT NOT NULL, 
    [member_id] INT NOT NULL, 
    [refill_amount] DECIMAL(10, 2) NOT NULL, 
    [staff_id] INT NOT NULL, 
    [refill_time] DATETIME NOT NULL, 
    CONSTRAINT [FK_member_refill_branch] FOREIGN KEY ([branch_id]) REFERENCES [branch]([id]), 
    CONSTRAINT [FK_member_refill_member] FOREIGN KEY ([member_id]) REFERENCES [member]([id]), 
    CONSTRAINT [FK_member_refill_staff] FOREIGN KEY ([staff_id]) REFERENCES [staff]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'充入金额',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'member_refill',
    @level2type = N'COLUMN',
    @level2name = N'refill_amount'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'那个分店充值',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'member_refill',
    @level2type = N'COLUMN',
    @level2name = N'branch_id'
GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'充值时间',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'member_refill',
    @level2type = N'COLUMN',
    @level2name = N'refill_time'