CREATE TABLE [dbo].[setmeal_item_opt]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [setmeal_item_id] INT NOT NULL, 
    [menu_id] INT NOT NULL, 
    CONSTRAINT [FK_setmeal_item_opt_setmeal_item] FOREIGN KEY ([setmeal_item_id]) REFERENCES [menu]([id]), 
    CONSTRAINT [FK_setmeal_item_opt_menu] FOREIGN KEY ([menu_id]) REFERENCES [menu]([id]),
)
