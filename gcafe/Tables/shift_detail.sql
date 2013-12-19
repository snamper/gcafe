CREATE TABLE [dbo].[shift_detail]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [shift_id] INT NOT NULL, 
    [order_id] INT NOT NULL, 
    CONSTRAINT [FK_shift_detail_shift] FOREIGN KEY ([shift_id]) REFERENCES [shift]([id]), 
    CONSTRAINT [FK_shift_detail_order] FOREIGN KEY ([order_id]) REFERENCES [order]([id]),
)
