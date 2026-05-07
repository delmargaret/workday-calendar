using NUnit.Framework;
using WorkdayCalendar.Domain.Holidays;

namespace WorkdayCalendar.Domain.Tests;

public class RecurringHolidayTests
{
    [Test]
    public void Constructor_allows_february_twenty_ninth()
    {
        var holiday = new RecurringHoliday(29, 2);

        Assert.That(holiday.Day, Is.EqualTo(29));
        Assert.That(holiday.Month, Is.EqualTo(2));
    }

    [Test]
    public void Constructor_rejects_invalid_day()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecurringHoliday(30, 2));
    }

    [Test]
    public void Constructor_rejects_invalid_month()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() => new RecurringHoliday(23, 13));
    }
}
