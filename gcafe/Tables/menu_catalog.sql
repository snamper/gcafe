CREATE TABLE [dbo].[menu_catalog]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [name] NVARCHAR(10) NOT NULL, 
    [parent_id] INT NULL
)
