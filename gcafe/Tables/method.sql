CREATE TABLE [dbo].[method]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
	[method_catalog_id] INT NULL, 
    [name] NVARCHAR(10) NOT NULL, 
    CONSTRAINT [FK_method_method_catalog] FOREIGN KEY ([method_catalog_id]) REFERENCES [method_catalog]([id]),
)

GO
EXEC sp_addextendedproperty @name = N'MS_Description',
    @value = N'做法名称',
    @level0type = N'SCHEMA',
    @level0name = N'dbo',
    @level1type = N'TABLE',
    @level1name = N'method',
    @level2type = N'COLUMN',
    @level2name = N'name'