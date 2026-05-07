namespace WorkdayCalendar.Domain.Holidays;

/// <summary>
/// Represents a holiday that occurs on the same day and month every year.
/// </summary>
public record RecurringHoliday
{
    /// <summary>
    /// Initializes a new instance of the <see cref="RecurringHoliday" /> class.
    /// </summary>
    /// <param name="day">The day of month.</param>
    /// <param name="month">The month.</param>
    public RecurringHoliday(int day, int month)
    {
        Day = day;
        Month = month;
        Validate();
    }

    /// <summary>
    /// Gets the day of month.
    /// </summary>
    public int Day { get; }

    /// <summary>
    /// Gets the month.
    /// </summary>
    public int Month { get; }

    private void Validate()
    {
        // 2024 leap year
        _ = new DateOnly(2024, Month, Day);
    }
}
