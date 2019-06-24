using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminder.Storage.Core;
using System;
using System.Linq;

namespace Reminder.Storage.DbStorage.Tests
{
	[TestClass]
	public class DataBaseReminderStorageTests
	{
		private readonly string _connectionString =
			@"Data Source=localhost\SQLEXPRESS;Initial Catalog=RemindersDBTest;Integrated Security=true;";

		[TestInitialize]
		public void TestInitialize()
		{
			new DataBaseReminderStorageInit(_connectionString)
				.InitializeDatabase();
		}

		[TestMethod]
		public void Add_Method_Returns_Not_Empty_Guid()
		{
			var storage = new DataBaseReminderStorage(_connectionString);

			var guid = storage.Add(new Core.ReminderItemRestricted()
			{
				Date = DateTimeOffset.Now.AddHours(1),
				ContactId = "12345",
				Message = "testMessage",
				Status = Core.ReminderItemStatus.Awaiting
			});

			Assert.AreNotEqual(Guid.Empty, guid);
		}

		[TestMethod]
		public void Count_Property_Returns_10_With_10_Test_Rows()
		{
			var storage = new DataBaseReminderStorage(_connectionString);

			int actual = storage.Count;

			Assert.AreEqual(10, actual);
		}

		[TestMethod]
		public void Clear_Method_Clears_Database()
		{
			var storage = new DataBaseReminderStorage(_connectionString);

			storage.Clear();

			var actual = storage.Count;

			Assert.AreEqual(0, actual);
		}

		[TestMethod]
		public void Get_By_Id_Method_Returns_Exact_Reminder_Item()
		{
			var storage = new DataBaseReminderStorage(_connectionString);

			var restrictedReminder = new Core.ReminderItemRestricted()
			{
				Date = DateTimeOffset.Parse("2019-10-10T00:00+00:00"),
				ContactId = "12345",
				Message = "testMessage",
				Status = Core.ReminderItemStatus.Awaiting
			};

			var guid = storage.Add(restrictedReminder);

			var fullReminder = storage.Get(guid);

			Assert.AreEqual(restrictedReminder.ContactId, fullReminder.ContactId);
			Assert.AreEqual(restrictedReminder.Date, fullReminder.Date);
			Assert.AreEqual(restrictedReminder.Message, fullReminder.Message);
			Assert.AreEqual(restrictedReminder.Status, fullReminder.Status);
		}

		[TestMethod]
		public void Get_By_Id_Method_Returns_Null_With_Not_Existing_Guid()
		{
			var storage = new DataBaseReminderStorage(_connectionString);

			var nullReminder = storage.Get(Guid.Empty);

			Assert.IsNull(nullReminder);
		}

		[TestMethod]
		public void Get_By_Status_Method_Returns_Valid_Count_Of_Reminders_With_Test_Data()
		{
			var storage = new DataBaseReminderStorage(_connectionString);

			var awaitingReminders = storage.Get(ReminderItemStatus.Awaiting);
			var readyReminders = storage.Get(ReminderItemStatus.Ready);
			var sentReminders = storage.Get(ReminderItemStatus.Sent);
			var failedReminders = storage.Get(ReminderItemStatus.Failed);


			Assert.AreEqual(5, awaitingReminders.Count);
			Assert.AreEqual(2, readyReminders.Count);
			Assert.AreEqual(2, sentReminders.Count);
			Assert.AreEqual(1, failedReminders.Count);
		}

		[TestMethod]
		public void UpdateStatus_By_ONE_Id_Method_Updates_Status()
		{
			var storage = new DataBaseReminderStorage(_connectionString);

			var restrictedReminder = new Core.ReminderItemRestricted()
			{
				Date = DateTimeOffset.Parse("2019-10-10T00:00+00:00"),
				ContactId = "12345",
				Message = "testMessage",
				Status = Core.ReminderItemStatus.Awaiting
			};

			var guid = storage.Add(restrictedReminder);

			Assert.AreEqual(
				ReminderItemStatus.Awaiting,
				storage.Get(guid).Status);

			storage.UpdateStatus(guid, ReminderItemStatus.Ready);

			Assert.AreEqual(
				ReminderItemStatus.Ready,
				storage.Get(guid).Status);
		}

		[TestMethod]
		public void UpdateStatus_By_SEVERAL_Ids_Method_Updates_Status()
		{
			var storage = new DataBaseReminderStorage(_connectionString);

			// we know that added 5 test awaiting reminders

			var awaitingRemindersIds =
				storage
				.Get(ReminderItemStatus.Awaiting)
				.Select(x => x.Id);

			storage.UpdateStatus(
				awaitingRemindersIds,
				ReminderItemStatus.Failed);

			// also we know that there is 1 more test failed reminder

			Assert.AreEqual(
				6,
				storage.Get(ReminderItemStatus.Failed).Count);
		}


	}
}
