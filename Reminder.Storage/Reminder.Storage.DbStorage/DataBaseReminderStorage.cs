using System;
using System.Collections.Generic;
using Reminder.Storage.Core;
using System.Data.SqlClient;

namespace Reminder.Storage.DbStorage
{
	public class DataBaseReminderStorage : IReminderStorage
	{
		private const string _connectionString =
            @"Data Source=localhost\SQLEXPRESS;" +
			"Initial Catalog=RemindersDB;" +
			"Integrated Security=true;";

		public int Count
		{
			get
			{
				using (var sqlConnection = GetOpenedSqlConnection())
				{
					var sqlCommand = sqlConnection.CreateCommand();
					sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
					sqlCommand.CommandText = "dbo.GetRemindersCount";

					var countParameter = new SqlParameter("@count", System.Data.SqlDbType.Int);
					countParameter.Direction = System.Data.ParameterDirection.Output;

					sqlCommand.Parameters.Add(countParameter);

					sqlCommand.ExecuteNonQuery();

					var count = (int)countParameter.Value;

					return count;
				}
			}
		}

		public Guid Add(ReminderItemRestricted reminder)
		{
			using (var sqlConnection = GetOpenedSqlConnection())
			{
				var sqlCommand = sqlConnection.CreateCommand();
				sqlCommand.CommandType = System.Data.CommandType.StoredProcedure;
				sqlCommand.CommandText = "dbo.CreateReminder";

				sqlCommand.Parameters.AddWithValue("@targetDate", reminder.Date);
				sqlCommand.Parameters.AddWithValue("@chat", reminder.ContactId);
				sqlCommand.Parameters.AddWithValue("@message", reminder.Message);
				sqlCommand.Parameters.AddWithValue("@status", reminder.Status.ToString());

				var idParameter = new SqlParameter("@id", System.Data.SqlDbType.UniqueIdentifier);
				idParameter.Direction = System.Data.ParameterDirection.Output;

				sqlCommand.Parameters.Add(idParameter);

				sqlCommand.ExecuteNonQuery();

				var id = (Guid)idParameter.Value;

				return id;
			}
		}

		public void Clear()
		{
			using (var connection = GetOpenedSqlConnection())
			{
				var command = connection.CreateCommand();
				command.CommandType = System.Data.CommandType.StoredProcedure;
				command.CommandText = "dbo.ClearReminders";

				command.ExecuteNonQuery();
			}
		}

		public ReminderItem Get(Guid id)
		{
			using (var connection = GetOpenedSqlConnection())
			{
				var command = connection.CreateCommand();
				command.CommandType = System.Data.CommandType.StoredProcedure;
				command.CommandText = "dbo.GetReminderById";

				var idParameter = new SqlParameter()
				{
					ParameterName = "@id",
					DbType = System.Data.DbType.Guid,
					Value = id
				};

				var targetDateParameter = new SqlParameter()
				{
					ParameterName = "@targetDate",
					DbType = System.Data.DbType.DateTimeOffset,
					Direction = System.Data.ParameterDirection.Output
				};

				var messageParameter = new SqlParameter()
				{
					ParameterName = "@message",
					DbType = System.Data.DbType.String,
					Size = 300,
					Direction = System.Data.ParameterDirection.Output
				};

				var chatParameter = new SqlParameter()
				{
					ParameterName = "@chat",
					DbType = System.Data.DbType.String,
					Size = 15,
					Direction = System.Data.ParameterDirection.Output
				};

				var statusIdParameter = new SqlParameter()
				{
					ParameterName = "@statusId",
					DbType = System.Data.DbType.Byte,
					Direction = System.Data.ParameterDirection.Output
				};

				command.Parameters.Add(idParameter);
				command.Parameters.Add(targetDateParameter);
				command.Parameters.Add(messageParameter);
				command.Parameters.Add(chatParameter);
				command.Parameters.Add(statusIdParameter);

				command.ExecuteNonQuery();

				if (statusIdParameter.Value == DBNull.Value)
					return null;

				ReminderItemStatus status =
					(ReminderItemStatus)Enum.Parse(
						typeof(ReminderItemStatus),
						statusIdParameter.Value.ToString());

				return new ReminderItem()
				{
					Id = (Guid)idParameter.Value,
					Date = (DateTimeOffset)targetDateParameter.Value,
					Message = (string)messageParameter.Value,
					ContactId = (string)chatParameter.Value,
					Status = status
				};
			}
		}

		public List<ReminderItem> Get(int count = 0, int startPosition = 0)
		{
			return null;
		}

		public List<ReminderItem> Get(ReminderItemStatus status, int count, int startPosition)
		{
			return null;
		}

		public List<ReminderItem> Get(ReminderItemStatus status)
		{
			var result = new List<ReminderItem>();

			using (var connection = GetOpenedSqlConnection())
			{
				var command = connection.CreateCommand();
				command.CommandType = System.Data.CommandType.StoredProcedure;
				command.CommandText = "dbo.GetRemindersByLastStatus";

				command.Parameters.AddWithValue("@status", status.ToString());

				using(var reader = command.ExecuteReader())
				{
					if (!reader.HasRows)
						return result;

					int idColumnIndex = reader.GetOrdinal("Id");
					int targetDateColumnIndex = reader.GetOrdinal("TargetDate");
					int chatColumnIndex = reader.GetOrdinal("Chat");
					int messageColumnIndex = reader.GetOrdinal("Message");
					int statusColumnIndex = reader.GetOrdinal("Status");

					while (reader.Read())
					{
						var id = reader.GetGuid(idColumnIndex);
						var date = reader.GetDateTimeOffset(targetDateColumnIndex);
						var contactId = reader.GetString(chatColumnIndex);
						var message = reader.GetString(messageColumnIndex);
						var rStatus = reader.GetString(statusColumnIndex);

						result.Add(
							new ReminderItem()
							{
								Id = id,
								Date = date,
								ContactId = contactId,
								Message = message,
								Status = (ReminderItemStatus)Enum.Parse(typeof(ReminderItemStatus), rStatus)
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
				command.CommandType = System.Data.CommandType.StoredProcedure;
				command.CommandText = "dbo.AddReminderStatus";

				command.Parameters.AddWithValue("@status", status.ToString());

				foreach(var id in ids)
				{
					command.Parameters.AddWithValue("@reminderId", id);
					command.ExecuteNonQuery();
					command.Parameters.RemoveAt("@reminderId");
				}
			}
		}

		public void UpdateStatus(Guid id, ReminderItemStatus status)
		{
			using(var connection = GetOpenedSqlConnection())
			{
				var command = connection.CreateCommand();
				command.CommandType = System.Data.CommandType.StoredProcedure;
				command.CommandText = "dbo.AddReminderStatus";

				command.Parameters.AddWithValue("@reminderId", id);
				command.Parameters.AddWithValue("@status", status.ToString());

				command.ExecuteNonQuery();
			}
		}

		private SqlConnection GetOpenedSqlConnection()
		{
			var sqlConnection = new SqlConnection(_connectionString);
			sqlConnection.Open();
			return sqlConnection;
		}
	}
}
