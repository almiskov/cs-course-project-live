using Reminder.Storage.Core;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Reminder.Storage.SqlServer.ADO
{
    public class SqlReminderStorage : IReminderStorage
    {
        private readonly string _connectionString;

        public SqlReminderStorage(string connectionString)
        {
            _connectionString = connectionString;
        }
        public int Count => throw new NotImplementedException();

        public Guid Add(ReminderItemRestricted reminder)
        {
            using(var connection = GetOpenedSqlConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.AddReminderItem";

                command.Parameters.AddWithValue("@contactId", reminder.ContactId);
                command.Parameters.AddWithValue("@targetDate", reminder.Date);
                command.Parameters.AddWithValue("@message", reminder.Message);
                command.Parameters.AddWithValue("@statusId", (byte)reminder.Status);

                var reminderIdParameter = new SqlParameter()
                {
                    ParameterName = "@reminderId",
                    DbType = DbType.Guid,
                    Direction = ParameterDirection.Output
                };

                command.Parameters.Add(reminderIdParameter);

                command.ExecuteNonQuery();

                return (Guid)reminderIdParameter.Value;
            }
        }

        public void Clear()
        {
            throw new NotImplementedException();
        }

        public ReminderItem Get(Guid id)
        {
            throw new NotImplementedException();
        }

        public List<ReminderItem> Get(int count = 0, int startPosition = 0)
        {
            throw new NotImplementedException();
        }

        public List<ReminderItem> Get(ReminderItemStatus status, int count, int startPosition)
        {
            throw new NotImplementedException();
        }

        public List<ReminderItem> Get(ReminderItemStatus status)
        {
            throw new NotImplementedException();
        }

        public bool Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(IEnumerable<Guid> ids, ReminderItemStatus status)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(Guid id, ReminderItemStatus status)
        {
            throw new NotImplementedException();
        }

        private SqlConnection GetOpenedSqlConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            return connection;
        }
    }
}
