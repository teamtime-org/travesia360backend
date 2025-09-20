using TeamTime.Domain.Common;
using TeamTime.Domain.Enums;

namespace TeamTime.Domain.Entities;

public class TimePeriod : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public PeriodType Type { get; private set; }
    public DateTime StartDate { get; private set; }
    public DateTime EndDate { get; private set; }
    public decimal ReferenceHours { get; private set; }
    public bool IsActive { get; private set; } = true;
    public bool IsCurrent { get; private set; } = false;

    // Navigation properties
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

    // Constructors
    private TimePeriod() { } // EF Core constructor

    public TimePeriod(
        string name,
        string description,
        PeriodType type,
        DateTime startDate,
        DateTime endDate,
        decimal referenceHours)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        Type = type;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        ReferenceHours = referenceHours;
        IsActive = true;
        IsCurrent = false;

        ValidatePeriodDates();
        ValidateReferenceHours();
    }

    // Domain methods
    public void UpdatePeriod(
        string name,
        string description,
        DateTime startDate,
        DateTime endDate,
        decimal referenceHours)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        StartDate = startDate.Date;
        EndDate = endDate.Date;
        ReferenceHours = referenceHours;
        UpdatedAt = DateTime.UtcNow;

        ValidatePeriodDates();
        ValidateReferenceHours();
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        IsCurrent = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetAsCurrent()
    {
        if (!IsActive)
        {
            throw new InvalidOperationException("Cannot set inactive period as current");
        }

        IsCurrent = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveFromCurrent()
    {
        IsCurrent = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool ContainsDate(DateTime date)
    {
        var dateOnly = date.Date;
        return dateOnly >= StartDate && dateOnly <= EndDate;
    }

    public bool OverlapsWith(DateTime startDate, DateTime endDate)
    {
        var start = startDate.Date;
        var end = endDate.Date;

        return StartDate <= end && EndDate >= start;
    }

    public int GetWorkingDays()
    {
        var workingDays = 0;
        var currentDate = StartDate;

        while (currentDate <= EndDate)
        {
            if (currentDate.DayOfWeek != DayOfWeek.Saturday &&
                currentDate.DayOfWeek != DayOfWeek.Sunday)
            {
                workingDays++;
            }
            currentDate = currentDate.AddDays(1);
        }

        return workingDays;
    }

    public decimal GetDailyReferenceHours()
    {
        var workingDays = GetWorkingDays();
        return workingDays > 0 ? ReferenceHours / workingDays : 0;
    }

    public decimal GetWeeklyReferenceHours()
    {
        return Type switch
        {
            PeriodType.WEEKLY => ReferenceHours,
            PeriodType.BIWEEKLY => ReferenceHours / 2,
            PeriodType.MONTHLY => ReferenceHours / 4,
            _ => ReferenceHours
        };
    }

    public bool IsCurrentPeriod()
    {
        var today = DateTime.UtcNow.Date;
        return IsActive && IsCurrent && ContainsDate(today);
    }

    public string GetPeriodIdentifier()
    {
        return Type switch
        {
            PeriodType.WEEKLY => $"W{GetWeekOfYear(StartDate):D2}-{StartDate.Year}",
            PeriodType.BIWEEKLY => $"BW{GetBiweekNumber():D2}-{StartDate.Year}",
            PeriodType.MONTHLY => $"M{StartDate.Month:D2}-{StartDate.Year}",
            _ => $"P{StartDate:yyyyMMdd}-{EndDate:yyyyMMdd}"
        };
    }

    // Private helper methods
    private void ValidatePeriodDates()
    {
        if (StartDate >= EndDate)
        {
            throw new ArgumentException("Start date must be before end date");
        }

        var daysDifference = (EndDate - StartDate).Days + 1;

        switch (Type)
        {
            case PeriodType.WEEKLY when daysDifference != 7:
                throw new ArgumentException("Weekly period must be exactly 7 days");
            case PeriodType.BIWEEKLY when daysDifference != 14:
                throw new ArgumentException("Biweekly period must be exactly 14 days");
            case PeriodType.MONTHLY when daysDifference < 28 || daysDifference > 31:
                throw new ArgumentException("Monthly period must be between 28 and 31 days");
        }
    }

    private void ValidateReferenceHours()
    {
        if (ReferenceHours <= 0)
        {
            throw new ArgumentException("Reference hours must be greater than zero");
        }

        if (ReferenceHours > 200) // Reasonable upper limit
        {
            throw new ArgumentException("Reference hours cannot exceed 200 hours per period");
        }
    }

    private static int GetWeekOfYear(DateTime date)
    {
        var day = date.DayOfYear;
        return ((day - 1) / 7) + 1;
    }

    private int GetBiweekNumber()
    {
        var weekNumber = GetWeekOfYear(StartDate);
        return ((weekNumber - 1) / 2) + 1;
    }

    // Static factory methods
    public static TimePeriod CreateWeeklyPeriod(DateTime startDate, decimal weeklyHours = 40)
    {
        var monday = GetMonday(startDate);
        var sunday = monday.AddDays(6);

        var name = $"Week {GetWeekOfYear(monday)} - {monday.Year}";
        var description = $"Weekly period from {monday:MMM dd} to {sunday:MMM dd}, {monday.Year}";

        return new TimePeriod(name, description, PeriodType.WEEKLY, monday, sunday, weeklyHours);
    }

    public static TimePeriod CreateBiweeklyPeriod(DateTime startDate, decimal biweeklyHours = 80)
    {
        var monday = GetMonday(startDate);
        var endSunday = monday.AddDays(13);

        var name = $"Biweek starting {monday:MMM dd} - {monday.Year}";
        var description = $"Biweekly period from {monday:MMM dd} to {endSunday:MMM dd}, {monday.Year}";

        return new TimePeriod(name, description, PeriodType.BIWEEKLY, monday, endSunday, biweeklyHours);
    }

    public static TimePeriod CreateMonthlyPeriod(DateTime startDate, decimal monthlyHours = 160)
    {
        var firstDay = new DateTime(startDate.Year, startDate.Month, 1);
        var lastDay = firstDay.AddMonths(1).AddDays(-1);

        var name = $"{firstDay:MMMM yyyy}";
        var description = $"Monthly period for {firstDay:MMMM yyyy}";

        return new TimePeriod(name, description, PeriodType.MONTHLY, firstDay, lastDay, monthlyHours);
    }

    private static DateTime GetMonday(DateTime date)
    {
        var daysFromMonday = (int)date.DayOfWeek - (int)DayOfWeek.Monday;
        if (daysFromMonday < 0) daysFromMonday += 7;

        return date.AddDays(-daysFromMonday).Date;
    }
}