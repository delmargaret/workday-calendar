namespace WorkdayCalendar.Domain.Workdays;

/// <summary>
/// Represents the start and stop time of a workday.
/// </summary>
public record WorkdaySchedule
{
    /// <summary>
    /// Initializes a new instance of the <see cref="WorkdaySchedule" /> class.
    /// </summary>
    /// <param name="startsAt">The time when a workday starts.</param>
    /// <param name="stopsAt">The time when a workday stops.</param>
    public WorkdaySchedule(TimeOnly startsAt, TimeOnly stopsAt)
    {
        StartsAt = startsAt;
        StopsAt = stopsAt;
        Validate();
    }

    /// <summary>
    /// Gets the time when a workday starts.
    /// </summary>
    public TimeOnly StartsAt { get; }

    /// <summary>
    /// Gets the time when a workday stops.
    /// </summary>
    public TimeOnly StopsAt { get; }

    /// <summary>
    /// Gets the duration of a workday.
    /// </summary>
    public TimeSpan WorkdayDuration => StopsAt - StartsAt;

    private void Validate()
    {
        if (StopsAt <= StartsAt)
        {
            throw new ArgumentException("Workday stop time must be later than start time.");
        }
    }
}
