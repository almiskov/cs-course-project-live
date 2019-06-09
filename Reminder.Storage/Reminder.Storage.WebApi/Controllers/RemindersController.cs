using System;
using System.Linq;
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

		[HttpHead]
		public IActionResult GetCount()
		{
			string countAsString = _reminderStorage.Count.ToString();

			Response.Headers.Add("X-Total-Count", countAsString);

			return Ok();
		}

		[HttpDelete]
		public IActionResult ClearStorage()
		{
			_reminderStorage.Clear();

			return Ok();
		}

		[HttpPost]
		public IActionResult CreateReminder([FromBody] ReminderItemCreateModel reminder)
		{
			if (reminder == null || !ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var reminderItem = reminder.ToReminderItemRestricted();
			Guid id = _reminderStorage.Add(reminderItem);

			return CreatedAtRoute(
				"GetReminder",
				new { id },
				new ReminderItemGetModel(id, reminderItem));
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
			[FromQuery(Name = "[filter]status")] ReminderItemStatus status
			)
		{
			var remindersItemGetModels = _reminderStorage
				.Get(status)
				.Select(x => new ReminderItemGetModel(x))
				.ToList();

			return Ok(remindersItemGetModels);
		}

		//[HttpGet]
		//public IActionResult GetRemindersWithJustPagination(
		//	[FromQuery(Name = "[pagination]count")] int count = 0,
		//	[FromQuery(Name = "[pagination]startPosition")] int startPosition = 0)
		//{
		//	var reminderItemGetModels = _reminderStorage
		//		.Get(count, startPosition)
		//		.Select(r => new ReminderItemGetModel(r));

		//	return Ok(reminderItemGetModels);
		//}

		[HttpPatch]
		public IActionResult UpdateRemindersStatus([FromBody]ReminderItemsUpdateModel reminderItemsUpdateModel)
		{
			if (reminderItemsUpdateModel == null || !ModelState.IsValid)
			{
				return BadRequest(ModelState);
			}

			var reminderItemModelToPatch = new ReminderItemUpdateModel();
			reminderItemsUpdateModel.PatchDocument.ApplyTo(reminderItemModelToPatch);

			_reminderStorage.UpdateStatus(
				reminderItemsUpdateModel.Ids,
				reminderItemModelToPatch.Status);

			return NoContent();
		}

		[HttpPatch("{id}")]
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

			var reminderItemModelToPatch = new ReminderItemUpdateModel();
			//{
			//	Status = reminderItem.Status
			//};

			patchDocument.ApplyTo(reminderItemModelToPatch);

			_reminderStorage.UpdateStatus(
				id,
				reminderItemModelToPatch.Status);

			return NoContent();
		}
	}
}
