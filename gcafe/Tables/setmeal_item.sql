CREATE TABLE [dbo].[setmeal_item]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [setmeal_menu_id] INT NOT NULL, 
    [setmeal_item_menu_id] INT NOT NULL, 
    CONSTRAINT [FK_setmeal_item_menu] FOREIGN KEY ([setmeal_menu_id]) REFERENCES [menu]([id]), 
    CONSTRAINT [FK_setmeal_item_setmeal_item] FOREIGN KEY ([setmeal_item_menu_id]) REFERENCES [menu]([id]),
)
