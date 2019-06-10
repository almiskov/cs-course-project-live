using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Reminder.Storage.SqlServer.ADO.Tests
{
    [TestClass]
    public class SqlReminderStorageTests
    {
        private const string _connectionString =
            @"Data Source=DESKTOP-QRP79DQ\SQLEXPRESS;Initial Catalog=ReminderTests;Integrated Security=true";

        [TestInitialize]
        public void TestInitialize()
        {
            new SqlReminderStorageInit(_connectionString).InitializeDatabase();
        }

        [TestMethod]
        public void Method_Add_Returns_Not_Empty_Guid()
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
    }
}
