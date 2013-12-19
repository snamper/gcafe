CREATE TABLE [dbo].[sys_info]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [branch_id] INT NOT NULL, 
    [order_cnt] INT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_sys_info_branch] FOREIGN KEY ([branch_id]) REFERENCES [branch]([id]),
)
