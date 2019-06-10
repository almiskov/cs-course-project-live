--1
INSERT INTO dbo.ReminderItem (
	Id,
	ContactId,
	TargetDate,
	[Message],
	StatusId,
	CreatedDate,
	UpdatedDate)
VALUES (
	'00000000-0000-0000-0000-111111111111',
	'ContactId_1',
	'2020-06-11 00:00:00+00:00',
	'TestMessage1',
	'0',
	'2020-06-10 00:00:00+00:00',
	'2020-06-10 00:00:00+00:00' )
--2
INSERT INTO dbo.ReminderItem (
	Id,
	ContactId,
	TargetDate,
	[Message],
	StatusId,
	CreatedDate,
	UpdatedDate)
VALUES (
	'00000000-0000-0000-0000-222222222222',
	'ContactId_1',
	'2020-06-21 00:00:00+00:00',
	'TestMessage2',
	'1',
	'2020-06-20 00:00:00+00:00',
	'2020-06-20 00:00:00+00:00' )
