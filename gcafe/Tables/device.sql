CREATE TABLE [dbo].[device]
(
	[id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [device_id] VARCHAR(100) NOT NULL, 
	[register_ticket] VARCHAR(100) NOT NULL,
    [register_time] DATETIME NOT NULL, 
    [is_deny] BIT NOT NULL DEFAULT 0, 
)
