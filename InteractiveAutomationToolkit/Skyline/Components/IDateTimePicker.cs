namespace Skyline.DataMiner.InteractiveAutomationToolkit
{
	using System;

	using Skyline.DataMiner.Automation;

	/// <summary>
	///     Represents a widget to show/edit date and time.
	/// </summary>
	public interface IDateTimePicker : ITimePickerBase
	{
		/// <summary>
		///     Triggered when a different datetime is picked.
		///     WantsOnChange will be set to true when this event is subscribed to.
		/// </summary>
		event EventHandler<DateTimePicker.ChangedEventArgs> Changed;

		/// <summary>
		///     Gets or sets a value indicating whether the calendar pop-up will close when the user clicks a new date.
		/// </summary>
		bool AutoCloseCalendar { get; set; }

		/// <summary>
		///     Gets or sets the display mode of the calendar inside the date-time picker control.
		///     Default: <c>CalendarMode.Month</c>.
		/// </summary>
		CalendarMode CalendarDisplayMode { get; set; }

		/// <summary>
		///     Gets or sets the time format string used when TimeFormat is set to <c>DateTimeFormat.Custom</c>.
		/// </summary>
		/// <remarks>Sets <see cref="TimeFormat" /> to <c>DateTimeFormat.Custom</c>.</remarks>
		string CustomTimeFormat { get; set; }

		/// <summary>
		///     Gets or sets the datetime displayed in the datetime picker.
		/// </summary>
		DateTime DateTime { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the displayed time is the server time or local time.
		/// </summary>
		bool DisplayServerTime { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the calendar control drop-down button is shown.
		///     Default: <c>true</c>.
		/// </summary>
		bool HasDropDownButton { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the spin box of the calender control is shown.
		///     Default: <c>true</c>.
		/// </summary>
		bool HasTimePickerSpinnerButton { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the spin box of the calender is enabled.
		///     Default: <c>true</c>.
		/// </summary>
		bool IsTimePickerSpinnerButtonEnabled { get; set; }

		/// <summary>
		///     Gets or sets a value indicating whether the time picker is shown within the calender control.
		///     Default: <c>true</c>.
		/// </summary>
		bool IsTimePickerVisible { get; set; }

		/// <summary>
		///     Gets or sets the maximum timestamp.
		/// </summary>
		DateTime Maximum { get; set; }

		/// <summary>
		///     Gets or sets the minimum timestamp.
		/// </summary>
		DateTime Minimum { get; set; }

		/// <summary>
		///     Gets or sets the time format of the time picker.
		///     Default: <c>DateTimeFormat.ShortTime</c>.
		/// </summary>
		DateTimeFormat TimeFormat { get; set; }

		/// <summary>
		///     Gets or sets the tooltip.
		/// </summary>
		/// <exception cref="ArgumentNullException">When the value is <c>null</c>.</exception>
		string Tooltip { get; set; }

		/// <summary>
		///     Gets or sets the state indicating if a given input field was validated or not and if the validation was valid.
		///     This should be used by the client to add a visual marker on the input field.
		/// </summary>
		/// <remarks>Available from DataMiner 10.0.5 onwards.</remarks>
		UIValidationState ValidationState { get; set; }

		/// <summary>
		///     Gets or sets the text that is shown if the validation state is invalid.
		///     This should be used by the client to add a visual marker on the input field.
		/// </summary>
		/// <remarks>Available from DataMiner 10.0.5 onwards.</remarks>
		string ValidationText { get; set; }
	}
}