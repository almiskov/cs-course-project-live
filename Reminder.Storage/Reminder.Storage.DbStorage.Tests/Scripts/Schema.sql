-- drop foreign keys
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Reminder]') AND type in (N'U'))
ALTER TABLE [dbo].[Reminder] DROP CONSTRAINT IF EXISTS [FK_Reminder_ChatId]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReminderStatus]') AND type in (N'U'))
ALTER TABLE [dbo].[ReminderStatus] DROP CONSTRAINT IF EXISTS [FK_ReminderStatus_ReminderId]
GO
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ReminderStatus]') AND type in (N'U'))
ALTER TABLE [dbo].[ReminderStatus] DROP CONSTRAINT IF EXISTS [FK_ReminderStatus_StatusId]
GO

-- (Re-)creating tables
DROP TABLE IF EXISTS [dbo].[Chat]
GO
CREATE TABLE dbo.Chat (
	Id INT NOT NULL IDENTITY(1,1),
	Chat VARCHAR(15) NOT NULL,
	CONSTRAINT PK_Chat PRIMARY KEY CLUSTERED (Id),
	CONSTRAINT UQ_Chat_Chat UNIQUE (Chat)
);
GO

DROP TABLE IF EXISTS [dbo].[Reminder]
GO
CREATE TABLE dbo.Reminder (
	Id UNIQUEIDENTIFIER NOT NULL CONSTRAINT DF_Reminder_Id DEFAULT NEWID(),
	CreationDate DATETIMEOFFSET(0) NOT NULL CONSTRAINT DF_Reminder_CreationDate DEFAULT SYSDATETIMEOFFSET(),
	TargetDate DATETIMEOFFSET(0) NOT NULL,
	[Message] VARCHAR(300) NOT NULL CONSTRAINT DF_Reminder_Message DEFAULT 'Smth',
	ChatId INT NOT NULL,
	CONSTRAINT PK_Reminder PRIMARY KEY CLUSTERED (Id)
);
GO

DROP TABLE IF EXISTS [dbo].[ReminderStatus]
GO
CREATE TABLE dbo.ReminderStatus (
	ReminderId UNIQUEIDENTIFIER NOT NULL,
	StatusId TINYINT NOT NULL,
	StatusChanged DATETIMEOFFSET(0) NOT NULL CONSTRAINT DF_StatusChanged DEFAULT SYSDATETIMEOFFSET(),
	CONSTRAINT PK_ReminderStatus PRIMARY KEY (ReminderId, StatusId),
	CONSTRAINT CK_StatusId CHECK (StatusId >= 0 AND StatusId <= 3)
);

DROP TABLE IF EXISTS [dbo].[Status]
GO
CREATE TABLE dbo.[Status] (
	Id TINYINT NOT NULL IDENTITY(0,1),
	[Status] VARCHAR(20) NOT NULL,
	CONSTRAINT PK_Status PRIMARY KEY CLUSTERED (Id)
);

-- Setting foreign keys
ALTER TABLE [dbo].[Reminder] WITH CHECK 
	ADD CONSTRAINT [FK_Reminder_ChatId] FOREIGN KEY([ChatId])
	REFERENCES [dbo].[Chat] ([Id])
GO

ALTER TABLE [dbo].[ReminderStatus]  WITH CHECK ADD  CONSTRAINT [FK_ReminderStatus_ReminderId] FOREIGN KEY([ReminderId])
REFERENCES [dbo].[Reminder] ([Id])
GO

ALTER TABLE [dbo].[ReminderStatus]  WITH CHECK ADD  CONSTRAINT [FK_ReminderStatus_StatusId] FOREIGN KEY([StatusId])
REFERENCES [dbo].[Status] ([Id])
GO


-- Inserting reminder status vocabulary
INSERT INTO dbo.[Status] ([Status])
VALUES ('Awaiting'), ('Ready'), ('Sent'), ('Failed')