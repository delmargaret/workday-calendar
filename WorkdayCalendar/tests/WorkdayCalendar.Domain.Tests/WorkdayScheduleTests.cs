using NUnit.Framework;
using WorkdayCalendar.Domain.Workdays;

namespace WorkdayCalendar.Domain.Tests;

public class WorkdayScheduleTests
{
    [Test]
    public void Constructor_rejects_stop_time_equal_to_start_time()
    {
        var exception = Assert.Throws<ArgumentException>(() => new WorkdaySchedule(new TimeOnly(8, 0), new TimeOnly(8, 0)));

        Assert.That(exception?.Message, Is.EqualTo("Workday stop time must be later than start time."));
    }

    [Test]
    public void Constructor_rejects_invalid_stop_and_start_times()
    {
        var exception = Assert.Throws<ArgumentException>(() => new WorkdaySchedule(new TimeOnly(16, 0), new TimeOnly(8, 0)));

        Assert.That(exception?.Message, Is.EqualTo("Workday stop time must be later than start time."));
    }

    [Test]
    public void WorkdayDuration_returns_difference_between_start_and_stop()
    {
        var workday = new WorkdaySchedule(new TimeOnly(8, 30), new TimeOnly(16, 0));

        Assert.That(workday.WorkdayDuration, Is.EqualTo(TimeSpan.FromHours(7.5)));
    }
}
