CREATE TABLE [dbo].[staff]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [number] VARCHAR(20) NULL, 
    [name] NVARCHAR(10) NOT NULL, 
    [password] VARCHAR(50) NULL, 
    [branch_id] INT NOT NULL, 
    [role_id] INT NOT NULL, 
	[join_date] DATE NOT NULL, 
    [is_deleted] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_staff_branch] FOREIGN KEY ([branch_id]) REFERENCES [branch]([id]), 
    CONSTRAINT [FK_staff_role] FOREIGN KEY ([role_id]) REFERENCES [role]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'员工记录不会删除，is_deleted表明不再可用',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'staff',
    @level2type = N'COLUMN',
    @level2name = N'is_deleted'