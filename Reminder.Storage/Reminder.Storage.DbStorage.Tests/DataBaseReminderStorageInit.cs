using System.Data.SqlClient;
using System.Text.RegularExpressions;
using Reminder.Storage.DbStorage.Tests.Properties;

namespace Reminder.Storage.DbStorage.Tests
{
	public class DataBaseReminderStorageInit
	{
		private readonly string _connectionString;

		public DataBaseReminderStorageInit(string connectionString)
		{
			_connectionString = connectionString;
		}

		public void InitializeDatabase()
		{
			RunSqlScript(Resources.Schema);
			RunSqlScript(Resources.SPs);
			RunSqlScript(Resources.Data);
		}

		private string[] SplitSqlInstructions(string script)
		{
			return Regex.Split(script, @"\bGO\b");
		}

		private void RunSqlScript(string script)
		{
			using(var connection = GetOpenedSqlConnection())
			{
				var command = connection.CreateCommand();
				command.CommandType = System.Data.CommandType.Text;

				var sqlInstructions = SplitSqlInstructions(script);

				foreach(var instruction in sqlInstructions)
				{
					if (string.IsNullOrWhiteSpace(instruction))
						continue;

					command.CommandText = instruction;
					command.ExecuteNonQuery();
				}
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
