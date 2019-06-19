DROP PROCEDURE IF EXISTS dbo.AddReminderItem
GO

CREATE PROCEDURE dbo.AddReminderItem(
	@contactId AS VARCHAR(50),
	@targetDate AS DATETIMEOFFSET,
	@message AS NVARCHAR(200),
	@statusId AS TINYINT,
	@reminderId AS UNIQUEIDENTIFIER OUTPUT
)
AS BEGIN
	SET NOCOUNT ON	

	DECLARE
		@tempReminderId AS UNIQUEIDENTIFIER,
		@now AS DATETIMEOFFSET

	SELECT
		@tempReminderId = NEWID(),
		@now = SYSDATETIMEOFFSET()
		
	INSERT INTO dbo.ReminderItem (
		[Id],
		[ContactId],
		[TargetDate],
		[Message],
		[StatusId],
		[CreatedDate],
		[UpdatedDate]
		)
	VALUES (
		@tempReminderId,
		@contactId,
		@targetDate,
		@message,
		@statusId,
		@now,
		@now
		)
	SET @reminderId = @tempReminderId
END
GO

DROP PROCEDURE IF EXISTS dbo.GetReminderItemById
GO
CREATE PROCEDURE dbo.GetReminderItemById (
	@reminderId AS UNIQUEIDENTIFIER
)
AS BEGIN
	SELECT r.Id, r.ContactId, r.TargetDate, r.[Message], r.StatusId
	FROM dbo.ReminderItem r
	WHERE r.Id = @reminderId
END
GO

DROP PROCEDURE IF EXISTS dbo.GetReminderItemByStatus
GO
CREATE PROCEDURE dbo.GetReminderItemByStatus (
	@statusId AS TINYINT
)
AS BEGIN
	SELECT r.Id, r.ContactId, r.[Message], r.TargetDate
	FROM dbo.ReminderItem r
	WHERE r.StatusId = @statusId
END
GO

DROP PROCEDURE IF EXISTS dbo.UpdateReminderItemStatusById
GO
CREATE PROCEDURE dbo.UpdateReminderItemStatusById (
	@id AS UNIQUEIDENTIFIER,
	@statusId AS TINYINT
)
AS BEGIN
	UPDATE dbo.ReminderItem
	SET
		StatusId = @statusId,
		UpdatedDate = SYSDATETIMEOFFSET()
	WHERE Id = @id
END
GO

DROP PROCEDURE IF EXISTS [dbo].[GetReminderItemsCount];
GO
CREATE PROCEDURE dbo.GetReminderItemsCount
AS BEGIN
	SELECT COUNT(*)
	FROM dbo.ReminderItem
END
GO

DROP PROCEDURE IF EXISTS dbo.RemoveReminderById
GO
CREATE PROCEDURE dbo.RemoveReminderById (
	@id AS UNIQUEIDENTIFIER
)
AS BEGIN
	SET NOCOUNT ON

	DELETE FROM dbo.ReminderItem
	WHERE Id = @id

	SELECT CAST(@@ROWCOUNT AS BIT)
END
GO


DROP PROCEDURE IF EXISTS dbo.UpdateReminderItemsBulk
GO
CREATE PROCEDURE dbo.UpdateReminderItemsBulk (
	@statusId AS TINYINT
)
AS BEGIN
	UPDATE R
	SET
		R.StatusId = @statusId,
		R.UpdatedDate = SYSDATETIMEOFFSET()
	FROM dbo.ReminderItem R
	INNER JOIN #ReminderItem AS T
	ON T.Id = R.Id
END
GO