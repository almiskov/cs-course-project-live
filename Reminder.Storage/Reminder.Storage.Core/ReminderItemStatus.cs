namespace Reminder.Storage.Core
{
	/// <summary>
	/// The status of the single reminder item.
	/// </summary>
	public enum ReminderItemStatus
	{
		/// <summary>
		/// Reminder queued and waits its time for sending.
		/// </summary>
		Awaiting = 1,

		/// <summary>
		/// Reminder's time has come. Now it is the queue for sending.
		/// </summary>
		Ready = 2,

		/// <summary>
		/// Reminder was sent successfully.
		/// </summary>
		Sent = 3,

		/// <summary>
		/// Something went wrong while sending attempt.
		/// </summary>
		Failed = 4
	}
}
