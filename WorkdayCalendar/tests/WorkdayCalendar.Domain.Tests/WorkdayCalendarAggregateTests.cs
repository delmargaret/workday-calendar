using System.Globalization;
using NUnit.Framework;
using WorkdayCalendar.Domain.Holidays;
using WorkdayCalendar.Domain.Workdays;

namespace WorkdayCalendar.Domain.Tests;

public class WorkdayCalendarAggregateTests
{
    [TestCase("2004-05-24T18:05:00", -5.5, "2004-05-14T12:00:00")]
    [TestCase("2004-05-24T19:03:00", 44.723656, "2004-07-27T13:47:00")]
    [TestCase("2004-05-24T18:03:00", -6.7470217, "2004-05-13T10:02:00")]
    [TestCase("2004-05-24T08:03:00", 12.782709, "2004-06-10T14:18:00")]
    [TestCase("2004-05-24T07:03:00", 8.276628, "2004-06-04T10:12:00")]
    [TestCase("2026-06-10T04:00:00", 0.5, "2026-06-10T12:00:00")]
    [TestCase("2026-06-10T18:00:00", 0.25, "2026-06-15T10:00:00")]
    [TestCase("2026-06-10T04:00:00", -0.25, "2026-06-09T14:00:00")]
    [TestCase("2026-06-10T18:00:00", -0.25, "2026-06-10T14:00:00")]
    [TestCase("2026-06-10T08:00:00", 0.25, "2026-06-10T10:00:00")]
    [TestCase("2026-06-10T08:00:00", -0.25, "2026-06-09T14:00:00")]
    [TestCase("2026-06-10T16:00:00", 0.25, "2026-06-15T10:00:00")]
    [TestCase("2026-06-10T16:00:00", -0.25, "2026-06-10T14:00:00")]
    [TestCase("2026-06-10T12:00:00", 0.25, "2026-06-10T14:00:00")]
    [TestCase("2026-06-10T12:00:00", -0.25, "2026-06-10T10:00:00")]
    [TestCase("2026-06-13T10:00:00", 0.25, "2026-06-15T10:00:00")]
    [TestCase("2026-06-13T10:00:00", -0.25, "2026-06-10T14:00:00")]
    [TestCase("2026-06-11T10:00:00", 0.25, "2026-06-15T10:00:00")]
    [TestCase("2026-06-11T10:00:00", -0.25, "2026-06-10T14:00:00")]
    [TestCase("2026-06-12T10:00:00", 0.25, "2026-06-15T10:00:00")]
    [TestCase("2026-06-12T10:00:00", -0.25, "2026-06-10T14:00:00")]
    [TestCase("2026-06-10T04:00:00", 0, "2026-06-10T08:00:00")]
    [TestCase("2026-06-10T18:00:00", 0, "2026-06-15T08:00:00")]
    [TestCase("2026-06-13T10:00:00", 0, "2026-06-15T08:00:00")]
    [TestCase("2026-06-11T10:00:00", 0, "2026-06-15T08:00:00")]
    [TestCase("2026-06-10T12:00:00", 0.000001, "2026-06-10T12:00:00")]
    public void CalculateWorkdays_returns_expected_workday_time(
        string start,
        double workdaysToAdd,
        string expected)
    {
        var calendar = WorkdayCalendarAggregate.Create(
            new WorkdaySchedule(new TimeOnly(8, 0), new TimeOnly(16, 0)),
            [new DateOnly(2004, 5, 27), new DateOnly(2026, 6, 11)],
            [new RecurringHoliday(17, 5), new RecurringHoliday(12, 6)]);

        var result = calendar.CalculateWorkdays(DateTime.Parse(start, CultureInfo.InvariantCulture), workdaysToAdd);

        Assert.That(result, Is.EqualTo(DateTime.Parse(expected, CultureInfo.InvariantCulture)));
    }
}
