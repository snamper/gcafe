CREATE TABLE [dbo].[menu_catalog]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [name] NVARCHAR(50) NOT NULL, 
    [parent_id] INT NULL, 
    [locked] BIT NOT NULL DEFAULT 0, 
    CONSTRAINT [FK_menu_catalog_this] FOREIGN KEY ([parent_id]) REFERENCES [menu_catalog]([id]),
)
