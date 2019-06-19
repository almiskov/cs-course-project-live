using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminder.Storage.Core;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Reminder.Storage.SqlServer.ADO.Tests
{
    [TestClass]
    public class SqlReminderStorageTests
    {
        private const string _connectionString =
            @"Data Source=localhost\SQLEXPRESS;Initial Catalog=ReminderTests;Integrated Security=true";

        [TestInitialize]
        public void TestInitialize()
        {
            new SqlReminderStorageInit(_connectionString).InitializeDatabase();
        }

        [TestMethod]
        public void Add_Method_Returns_Not_Empty_Guid()
        {
            var storage = new SqlReminderStorage(_connectionString);

            Guid actual = storage.Add(new Core.ReminderItemRestricted()
            {
                ContactId = "1141431",
                Date = DateTimeOffset.Now.AddHours(1),
                Message = "TestMessage",
                Status = Core.ReminderItemStatus.Awaiting
            });

            Assert.AreNotEqual(Guid.Empty, actual);
        }

        [TestMethod]
        public void Get_By_Id_Method_Returns_Just_Added_Item()
        {
            var storage = new SqlReminderStorage(_connectionString);

            DateTimeOffset expectedDate = DateTimeOffset.Now;
            string expectedContactId = "TestContactId";
            string expectedMessage = "TestMessageText";
            ReminderItemStatus expectedStatus = ReminderItemStatus.Awaiting;

            Guid id = storage.Add(
                new ReminderItemRestricted()
                {
                    ContactId = expectedContactId,
                    Date = expectedDate,
                    Message = expectedMessage,
                    Status = expectedStatus
                });

            Assert.AreNotSame(Guid.Empty, id);

            var actualItem = storage.Get(id);

            Assert.IsNotNull(actualItem);

            Assert.AreEqual(expectedContactId, actualItem.ContactId);
            Assert.AreEqual(expectedDate, actualItem.Date);
            Assert.AreEqual(expectedMessage, actualItem.Message);
            Assert.AreEqual(expectedStatus, actualItem.Status);
            Assert.AreEqual(id, actualItem.Id);

        }

        [TestMethod]
        public void Get_By_Id_Method_Returns_Null_If_Not_Fing()
        {
            var storage = new SqlReminderStorage(_connectionString);

            var actual = storage.Get(new Guid());

            Assert.IsNull(actual);
        }

        [TestMethod]
        public void Get_By_Status_1_Returns_4_Reminder()
        {
            var storage = new SqlReminderStorage(_connectionString);

            var reminders = storage.Get(ReminderItemStatus.Ready);

            Assert.AreEqual(4, reminders.Count);
        }

        [TestMethod]
        public void Get_By_Status_2_Returns_1_Reminder()
        {
            var storage = new SqlReminderStorage(_connectionString);

            var reminders = storage.Get(ReminderItemStatus.Sent);

            Assert.AreEqual(1, reminders.Count);
        }

        [TestMethod]
        public void Update_Status_By_Id_Updates_Status()
        {
            var storage = new SqlReminderStorage(_connectionString);

            Guid id = new Guid("00000000-0000-0000-0000-111111111111");

            storage.UpdateStatus(
                id,
                ReminderItemStatus.Ready);
                
            var reminder = storage.Get(id);

            Assert.AreEqual(ReminderItemStatus.Ready, reminder.Status);
        }

        [TestMethod]
        public void Count_Property_Returns_9_For_Initial_Data_Set()
        {
            var storage = new SqlReminderStorage(_connectionString);

            var actual = storage.Count;

            Assert.AreEqual(9, actual);
        }

        [TestMethod]
        public void Delete_Property_Returns_1_If_Reminder_Was_Deleted()
        {
            var storage = new SqlReminderStorage(_connectionString);

            var isDeleted = storage.Remove(Guid.Parse("00000000-0000-0000-0000-111111111111"));

            Assert.IsTrue(isDeleted);
        }

        [TestMethod]
        public void Delete_Property_Returns_0_If_Reminder_Was_NOT_Deleted()
        {
            var storage = new SqlReminderStorage(_connectionString);

            var isDeleted = storage.Remove(Guid.Parse("00000000-0000-0000-0000-123456789000"));

            Assert.IsFalse(isDeleted);
        }

        [TestMethod]
        public void Update_Status_With_Ids_Collection_Updates_Status()
        {
            var storage = new SqlReminderStorage(_connectionString);

            var ids = new List<Guid>
            {
                new Guid("00000000-0000-0000-0000-111111111111"),
                new Guid("00000000-0000-0000-0000-222222222222"),
                new Guid("00000000-0000-0000-0000-333333333333")
            };

            storage.UpdateStatus(ids, ReminderItemStatus.Failed);

            var actual = storage.Get(ReminderItemStatus.Failed);

            Assert.IsTrue(actual.Select(x => x.Id).Contains(ids[0]));
            Assert.IsTrue(actual.Select(x => x.Id).Contains(ids[1]));
            Assert.IsTrue(actual.Select(x => x.Id).Contains(ids[2]));
        }
    }
}
