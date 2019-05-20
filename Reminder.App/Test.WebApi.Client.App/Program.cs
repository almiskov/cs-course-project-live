using Reminder.Storage.Core;
using Reminder.Storage.WebApi.Client;
using System;

namespace Test.WebApi.Client.App
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new ReminderStorageWebApiClient("http://localhost:50933");

			for(int i = 0; i < 10; i++)
			{
				var reminderItem = new ReminderItem()
				{
					Date = DateTimeOffset.Now,
					ContactId = "TestContactId",
					Message = "TestMessage"
				};

				client.Add(reminderItem);
			}
		}
	}
}
