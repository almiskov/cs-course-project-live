using Microsoft.VisualStudio.TestTools.UnitTesting;
using Reminder.Storage.SqlServer.ADO.Tests.Properties;
using System.Data.SqlClient;
using System.Text.RegularExpressions;

namespace Reminder.Storage.SqlServer.ADO.Tests
{
    public class SqlReminderStorageInit
    {
        private readonly string _connectionString;

        public SqlReminderStorageInit(string connectionString)
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
            using (var connection = GetOpenedSqlConnection())
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
