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
			{
				return BadRequest(ModelState);
			}

			var reminderItem = reminder.ToReminderItemRestricted();
			Guid id = _reminderStorage.Add(reminderItem); // вот тут есть подозрение на то, что где-то подменяется айди

			return CreatedAtRoute(
				"GetReminder",
				new { id },
				new ReminderItemGetModel(id, reminderItem));
		}

		[HttpPatch]
		public IActionResult UpdateReminderStatus([FromBody]ReminderItemsUpdateModel remindersItemsUpdateModel)
		{
			if (remindersItemsUpdateModel == null || !ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var remindersItemModelToPatch = new ReminderItemUpdateModel();
			remindersItemsUpdateModel.PatchDocument.ApplyTo(remindersItemModelToPatch);

			_reminderStorage.UpdateStatus(
				remindersItemsUpdateModel.Ids,
				remindersItemModelToPatch.Status);

			return NoContent();
		}

		[HttpPatch("id")]
		public IActionResult UpdateReminderStatus(Guid id, [FromBody]JsonPatchDocument<ReminderItemUpdateModel> patchDocument)
		{
			var reminderItem = _reminderStorage.Get(id);

			if(reminderItem == null)
			{
				return BadRequest();
			}

			if (patchDocument == null || !ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var remindersItemModelToPatch = new ReminderItemUpdateModel()
			{
				Status = reminderItem.Status
			};

			patchDocument.ApplyTo(remindersItemModelToPatch);

			_reminderStorage.UpdateStatus(
				id,
				remindersItemModelToPatch.Status);

			return NoContent();
		}

		//[HttpPatch("{id}")]
		//public IActionResult UpdateStatus(Guid id, [FromBody]JsonPatchDocument<ReminderItemsUpdateModel> patch)
		//{
		//	if(patch == null)
		//	{
		//		return BadRequest();
		//	}

		//	var reminderToPatch =_reminderStorage.Get(id);

		//	if(reminderToPatch == null)
		//	{
		//		return BadRequest();
		//	}

		//	var patchModel = new ReminderItemsUpdateModel();

		//	patch.ApplyTo(patchModel);

		//	//reminderToPatch.Status = patchModel.Status;

		//	return NoContent();
		//}
	}
}
