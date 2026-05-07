namespace WorkdayCalendar.Domain.Holidays;

/// <summary>
/// Stores fixed and recurring holidays.
/// </summary>
public class HolidaySchedule
{
    private readonly HashSet<DateOnly> holidays = [];
    private readonly HashSet<RecurringHoliday> recurringHolidays = [];

    /// <summary>
    /// Gets the configured fixed holidays.
    /// </summary>
    public IReadOnlySet<DateOnly> Holidays => holidays.ToHashSet();

    /// <summary>
    /// Gets the configured recurring holidays.
    /// </summary>
    public IReadOnlySet<RecurringHoliday> RecurringHolidays => recurringHolidays.ToHashSet();

    /// <summary>
    /// Adds a fixed holiday.
    /// </summary>
    /// <param name="date">The holiday date.</param>
    public void AddHoliday(DateOnly date)
    {
        holidays.Add(date);
    }

    /// <summary>
    /// Removes a fixed holiday.
    /// </summary>
    /// <param name="date">The holiday date.</param>
    public void RemoveHoliday(DateOnly date)
    {
        holidays.Remove(date);
    }

    /// <summary>
    /// Adds a recurring holiday.
    /// </summary>
    /// <param name="holiday">The recurring holiday.</param>
    public void AddRecurringHoliday(RecurringHoliday holiday)
    {
        recurringHolidays.Add(holiday);
    }

    /// <summary>
    /// Removes a recurring holiday.
    /// </summary>
    /// <param name="holiday">The recurring holiday.</param>
    public void RemoveRecurringHoliday(RecurringHoliday holiday)
    {
        recurringHolidays.Remove(holiday);
    }

    /// <summary>
    /// Determines whether a date is configured as a holiday.
    /// </summary>
    /// <param name="date">The date to check.</param>
    /// <returns><see langword="true" /> when the date is a fixed or recurring holiday.</returns>
    public bool IsHoliday(DateOnly date)
    {
        return holidays.Contains(date) || recurringHolidays.Contains(new RecurringHoliday(date.Day, date.Month));
    }
}
