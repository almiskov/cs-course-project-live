using Reminder.Storage.Core;
using Reminder.Storage.WebApi.Client;
using System;

namespace Test.WebApi.Client.App
{
	class Program
	{
		static void Main(string[] args)
		{
			var client = new ReminderStorageWebApiClient("https://localhost:5001");

			var reminderItem = new ReminderItemRestricted()
			{
				Date = DateTimeOffset.Now,
				ContactId = "TestContactId",
				Message = "TestMessage"
			};

			// где-то здесь в методе Add в storage id присываивается не тот, который нужен
			var id = client.Add(reminderItem);

			Console.WriteLine("Adding done");

			var reminderItemFromStorage = client.Get(id);

			Console.WriteLine($"Reading done:\n" +
				$"{reminderItemFromStorage.Id}\n" +
				$"{reminderItemFromStorage.Date}\n" +
				$"{reminderItemFromStorage.ContactId}\n" +
				$"{reminderItemFromStorage.Message}\n");
			
		}
	}
}
