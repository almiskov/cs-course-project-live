using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Reminder.Storage.Core;
using Reminder.Storage.WebApi.Core;

namespace Reminder.Storage.WebApi.Controllers
{
	[Route("api/reminders")]
	[ApiController]
	public class RemindersController : ControllerBase
	{
		private IReminderStorage _reminderStorage;

		public RemindersController(IReminderStorage reminderStorage)
		{
			_reminderStorage = reminderStorage;
		}

		[HttpGet("{id}", Name = "GetReminder")]
		public IActionResult Get(Guid id)
		{
			var reminderItem = _reminderStorage.Get(id);

			if (reminderItem == null)
				return NotFound();

			return Ok(new ReminderItemGetModel(reminderItem));
		}

		[HttpGet]
		public IActionResult GetReminders(
			[FromQuery(Name = "[filter]status")]
			ReminderItemStatus status)
		{
			var remindersItemGetModels = _reminderStorage
				.Get(status)
				.Select(x => new ReminderItemGetModel(x))
				.ToList();

			return Ok(remindersItemGetModels);
		}

		[HttpPost]
		public IActionResult CreateReminder([FromBody] ReminderItemCreateModel reminder)
		{
			if (reminder == null || !ModelState.IsValid)
				return BadRequest(ModelState);

			var reminderItem = reminder.ToReminderItem();

			_reminderStorage.Add(reminderItem);

			return CreatedAtRoute(
				"GetReminder",
				new { id = reminderItem.Id },
				new ReminderItemGetModel(reminderItem));
		}

		[HttpPatch("{id}")]
		public IActionResult UpdateStatus(Guid id, [FromBody]JsonPatchDocument<ReminderItemPatchModel> patch)
		{
			if(patch == null)
			{
				return BadRequest();
			}

			var reminderToPatch =_reminderStorage.Get(id);

			if(reminderToPatch == null)
			{
				return BadRequest();
			}

			var patchModel = new ReminderItemPatchModel();

			patch.ApplyTo(patchModel);

			reminderToPatch.Status = patchModel.Status;

			return NoContent();
		}
	}
}
