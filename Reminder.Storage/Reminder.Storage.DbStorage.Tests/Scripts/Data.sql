-- fill tables with test data

DECLARE @id0 AS UNIQUEIDENTIFIER
DECLARE @id1 AS UNIQUEIDENTIFIER
DECLARE @id2 AS UNIQUEIDENTIFIER
DECLARE @id3 AS UNIQUEIDENTIFIER
DECLARE @id4 AS UNIQUEIDENTIFIER
DECLARE @id5 AS UNIQUEIDENTIFIER
DECLARE @id6 AS UNIQUEIDENTIFIER
DECLARE @id7 AS UNIQUEIDENTIFIER
DECLARE @id8 AS UNIQUEIDENTIFIER
DECLARE @id9 AS UNIQUEIDENTIFIER

EXECUTE dbo.CreateReminder '2020-01-10 00:00',	'testChat0',	'testMessage0',		'Awaiting',		@id0 OUTPUT
EXECUTE dbo.CreateReminder '2020-01-01 00:00',	'testChat1',	'testMessage1',		'Awaiting',		@id1 OUTPUT
EXECUTE dbo.CreateReminder '2020-01-02 00:00',	'testChat2',	'testMessage2',		'Awaiting',		@id2 OUTPUT
EXECUTE dbo.CreateReminder '2020-01-03 00:00',	'testChat3',	'testMessage3',		'Awaiting',		@id3 OUTPUT
EXECUTE dbo.CreateReminder '2020-01-04 00:00',	'testChat4',	'testMessage4',		'Awaiting',		@id4 OUTPUT
EXECUTE dbo.CreateReminder '2020-01-05 00:00',	'testChat5',	'testMessage5',		'Awaiting',		@id5 OUTPUT
EXECUTE dbo.CreateReminder '2020-01-06 00:00',	'testChat6',	'testMessage6',		'Awaiting',		@id6 OUTPUT
EXECUTE dbo.CreateReminder '2020-01-07 00:00',	'testChat7',	'testMessage7',		'Awaiting',		@id7 OUTPUT
EXECUTE dbo.CreateReminder '2020-01-08 00:00',	'testChat8',	'testMessage8',		'Awaiting',		@id8 OUTPUT
EXECUTE dbo.CreateReminder '2020-01-09 00:00',	'testChat9',	'testMessage9',		'Awaiting',		@id9 OUTPUT

EXECUTE dbo.AddReminderStatus @id0, 'Ready'
EXECUTE dbo.AddReminderStatus @id0, 'Sent'
EXECUTE dbo.AddReminderStatus @id4, 'Ready'
EXECUTE dbo.AddReminderStatus @id5, 'Ready'
EXECUTE dbo.AddReminderStatus @id7, 'Ready'
EXECUTE dbo.AddReminderStatus @id7, 'Failed'
EXECUTE dbo.AddReminderStatus @id9, 'Ready'
EXECUTE dbo.AddReminderStatus @id9, 'Sent'