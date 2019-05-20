using Newtonsoft.Json;
using Reminder.Storage.Core;
using Reminder.Storage.WebApi.Core;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Linq;
using Microsoft.AspNetCore.JsonPatch;

namespace Reminder.Storage.WebApi.Client
{
	public class ReminderStorageWebApiClient : IReminderStorage
	{
		private string _baseWebApiUrl;
		private HttpClient _httpClient;

		public ReminderStorageWebApiClient(string baseWebApiUrl)
		{
			_baseWebApiUrl = baseWebApiUrl;
			_httpClient = HttpClientFactory.Create();
		}

		public void Add(ReminderItem reminder)
		{
			string method = "POST";
			string relativeUrl = "/api/reminders";
			string content = JsonConvert.SerializeObject(
				new ReminderItemCreateModel(reminder));

			HttpRequestMessage request = new HttpRequestMessage(
				new HttpMethod(method),
				_baseWebApiUrl + relativeUrl);

			request.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
			request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));

			var result = _httpClient.SendAsync(request).Result;

			if (result.StatusCode != System.Net.HttpStatusCode.Created)
			{
				throw new Exception(
					$"Error: {result.StatusCode}. " +
					$"Content: {result.Content.ReadAsStringAsync().Result}");
			}
		}

		public ReminderItem Get(Guid id)
		{
			string method = "GET";
			string relativeUrl = $"/api/reminders/{id}";

			HttpRequestMessage request = new HttpRequestMessage(
				new HttpMethod(method),
				_baseWebApiUrl + relativeUrl);

			var result = _httpClient.SendAsync(request).Result;

			if (result.StatusCode != System.Net.HttpStatusCode.OK)
			{
				throw new Exception(
					$"Error: {result.StatusCode}. " +
					$"Content: {result.Content.ReadAsStringAsync().Result}");
			}

			string stringJsonResult = result.Content.ReadAsStringAsync().Result;

			var gotReminder = JsonConvert.DeserializeObject<ReminderItemGetModel>(stringJsonResult);

			var reminderItem = gotReminder.ToReminderItem();

			return reminderItem;
		}

		public List<ReminderItem> Get(ReminderItemStatus status)
		{
			string method = "GET";
			string relativeUrl = $"/api/reminders/?status={status}";

			HttpRequestMessage request = new HttpRequestMessage(
				new HttpMethod(method),
				_baseWebApiUrl + relativeUrl);

			var result = _httpClient.SendAsync(request).Result;

			if (result.StatusCode != System.Net.HttpStatusCode.OK)
			{
				throw new Exception(
					$"Error: {result.StatusCode}. " +
					$"Content: {result.Content.ReadAsStringAsync().Result}");
			}

			string stringJsonResult = result.Content.ReadAsStringAsync().Result;

			var gotReminders = JsonConvert.DeserializeObject<List<ReminderItemGetModel>>(stringJsonResult);

			var reminderItems = gotReminders
									.Select(x => x.ToReminderItem())
									.ToList();

			return reminderItems;
		}

		public void UpdateStatus(IEnumerable<Guid> ids, ReminderItemStatus status)
		{

		}

		public void UpdateStatus(Guid id, ReminderItemStatus status)
		{
			string method = "PATCH";
			string relativeUrl = $"/api/reminders/{id}";

			var patchDoc = new JsonPatchDocument<ReminderItemPatchModel>();
			patchDoc.Replace(r => r.Status, status);

			string content = JsonConvert.SerializeObject(patchDoc);

			HttpRequestMessage request = new HttpRequestMessage(
				new HttpMethod(method),
				_baseWebApiUrl + relativeUrl);

			request.Content = new StringContent(content, System.Text.Encoding.UTF8, "application/json");
			request.Headers.Accept.Add(new System.Net.Http.Headers.MediaTypeWithQualityHeaderValue("*/*"));

			var result = _httpClient.SendAsync(request).Result;

			if (result.StatusCode != System.Net.HttpStatusCode.NoContent)
			{
				throw new Exception(
					$"Error: {result.StatusCode}. " +
					$"Content: {result.Content.ReadAsStringAsync().Result}");
			}
		}
	}
}
