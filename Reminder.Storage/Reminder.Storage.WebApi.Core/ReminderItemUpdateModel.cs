using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.JsonPatch;
using Reminder.Storage.Core;

namespace Reminder.Storage.WebApi.Core
{
	public class ReminderItemUpdateModel
	{
		public ReminderItemStatus Status { get; set; }
	}
}
