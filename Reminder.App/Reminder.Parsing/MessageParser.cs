using System;
using System.Text.RegularExpressions;

namespace Reminder.Parsing
{
	public static class MessageParser
	{
		public static ParsedMessage Parse(string message)
		{
			ParsedMessage parsedMessage;

			if (TryParseTimeAndDate(message, out parsedMessage))
				return parsedMessage;
			if (TryParseAsTimeInterval(message, out parsedMessage))
				return parsedMessage;

			return null;
		}

		internal static bool TryParseAsTimeInterval(string message, out ParsedMessage parsedMessage)
		{
			Regex regex = new Regex(
				@"\bчерез \d+ (" +
				@"((с\b)|(сек\b)|(секунд\w*))|" +
				@"((м\b)|(мин\b)|(минут\w*))|" +
				@"((ч\b)|(час\w*))|" +
				@"((д\b)|(де?н\w*)))", RegexOptions.IgnoreCase);

			if (regex.IsMatch(message) && regex.Matches(message).Count == 1)
			{
				parsedMessage = new ParsedMessage();

				// Getting message from input message and setting it
				string parsingMessage = regex.Replace(message, string.Empty).Trim();

				while (true) // removing double spacing
				{
					if (parsingMessage.Contains("  "))
						parsingMessage = parsingMessage.Replace("  ", " ");
					else
						break;
				}

				parsedMessage.Message =
					string.IsNullOrWhiteSpace(parsingMessage)
					? ParsedMessage.DefaultMessage
					: parsingMessage;

				// Getting target date-time from input message and setting it

				string parsingInterval = regex.Match(message).Value;

				double interval = Convert.ToDouble(
					Regex.Match(regex.Match(message).Value, @"\d+").Value);

				char unit = parsingInterval.Split(' ')[2][0]; // жёстко, но для этого паттерна норм =)

				TimeSpan timeSpan = TimeSpan.Zero;

				switch (unit)
				{
					case 'с':
						timeSpan = TimeSpan.FromSeconds(interval);
						break;
					case 'м':
						timeSpan = TimeSpan.FromMinutes(interval);
						break;
					case 'ч':
						timeSpan = TimeSpan.FromHours(interval);
						break;
					case 'д':
						timeSpan = TimeSpan.FromDays(interval);
						break;
				}

				parsedMessage.Date = DateTimeOffset.Now + timeSpan;

				return true;
			}
			else
			{
				parsedMessage = null;

				return false;
			}
		}

		internal static bool TryParseTimeAndDate(string message, out ParsedMessage parsedMessage)
		{
			Regex regexDay = new Regex(
				@"\b(?:0?[1-9]|1[0-9]|2[0-9]|3[0-1])(\.|-)(?:0?[1-9]|1[0-2])(\.|-)(20)?[0-9][0-9]\b",
				RegexOptions.IgnoreCase);

			Regex regexTime = new Regex(
				@"\b(в )?(?:0?[0-9]|1[0-9]|2[0-3]):[0-5][0-9]\b",
				RegexOptions.IgnoreCase);

			parsedMessage = new ParsedMessage();

			string day;
			string time;

			if (regexDay.IsMatch(message) && regexDay.Matches(message).Count == 1)
			{
				// Getting day from input message

				day = regexDay.Match(message).Value;

				if (!DateTimeOffset.TryParse(day, out DateTimeOffset result))
				{
					parsedMessage = null;
					return false;
				}
			}
			else
			{
				parsedMessage = null;
				return false;
			}

			if (regexTime.IsMatch(message) && regexTime.Matches(message).Count == 1)
			{
				// Getting time from input message

				time = regexTime.Match(message).Value;

				if (time.ToLower().StartsWith("в"))
					time = time.Substring(2);

				if (!DateTimeOffset.TryParse(time, out DateTimeOffset result))
				{
					parsedMessage = null;
					return false;
				}
			}
			else
			{
				parsedMessage = null;
				return false;
			}

			// Getting message from input message

			string parsingMessage =
				regexDay.Replace(
					regexTime.Replace(
						message, string.Empty),
											string.Empty).Trim();

			while (true)
			{
				if (parsingMessage.Contains("  "))
					parsingMessage = parsingMessage.Replace("  ", " ");
				else
					break;
			}

			// Setting properties to parsing message

			parsedMessage.Date =
				DateTimeOffset.Parse(day + " " + time);

			parsedMessage.Message =
				string.IsNullOrWhiteSpace(parsingMessage)
				? ParsedMessage.DefaultMessage
				: parsingMessage;

			return true;
		}
	}
}
