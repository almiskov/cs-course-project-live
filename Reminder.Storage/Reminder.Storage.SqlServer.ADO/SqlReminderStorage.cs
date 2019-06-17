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
            using (var connection = GetOpenedSqlConnection())
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
            using (var connection = GetOpenedSqlConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.GetReminderItemById";

                command.Parameters.AddWithValue("@reminderId", id);

                using (var reader = command.ExecuteReader())
                {
                    if (!reader.HasRows || !reader.Read())
                        return null;

                    var result = new ReminderItem();

                    result.Id = id;
                    result.ContactId = reader.GetString(reader.GetOrdinal("ContactId"));
                    result.Date = reader.GetDateTimeOffset(reader.GetOrdinal("TargetDate"));
                    result.Message = reader.GetString(reader.GetOrdinal("Message"));
                    result.Status = (ReminderItemStatus)reader.GetByte(reader.GetOrdinal("StatusId"));

                    return result;
                }
            }
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
            using (var connection = GetOpenedSqlConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.GetReminderItemByStatus";

                command.Parameters.AddWithValue("@statusId", status);

                using (var reader = command.ExecuteReader())
                {
                    var result = new List<ReminderItem>();

                    if (!reader.HasRows)
                        return result;

                    while (reader.Read())
                    {
                        result.Add(
                            new ReminderItem
                            {
                                Id = reader.GetGuid(reader.GetOrdinal("Id")),
                                ContactId = reader.GetString(reader.GetOrdinal("ContactId")),
                                Date = reader.GetDateTimeOffset(reader.GetOrdinal("TargetDate")),
                                Message = reader.GetString(reader.GetOrdinal("Message")),
                                Status = status
                            });
                    }

                    return result;
                }
            }
        }

        public bool Remove(Guid id)
        {
            throw new NotImplementedException();
        }

        public void UpdateStatus(IEnumerable<Guid> ids, ReminderItemStatus status)
        {
            using (var connection = GetOpenedSqlConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.UpdateReminderItemStatusById";

                command.Parameters.AddWithValue("@statusId", status);

                foreach(var id in ids)
                {
                    command.Parameters.AddWithValue("@id", id);
                    command.ExecuteNonQuery();
                    command.Parameters.RemoveAt("@id");
                }
            }
        }

        public void UpdateStatus(Guid id, ReminderItemStatus status)
        {
            using (var connection = GetOpenedSqlConnection())
            {
                var command = connection.CreateCommand();
                command.CommandType = CommandType.StoredProcedure;
                command.CommandText = "dbo.UpdateReminderItemStatusById";

                command.Parameters.AddWithValue("@id", id);
                command.Parameters.AddWithValue("@statusId", (byte)status);

                command.ExecuteNonQuery();
            }
        }

        private SqlConnection GetOpenedSqlConnection()
        {
            var connection = new SqlConnection(_connectionString);
            connection.Open();

            return connection;
        }
    }
}
