using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Services;

public interface ITimeEntryValidationService
{
    Task<ValidationResult> ValidateTimeEntryAsync(Guid userId, Guid projectId, DateOnly date, decimal hours, Guid? timePeriodId = null);
    Task<ValidationResult> ValidateTimeEntryUpdateAsync(Guid timeEntryId, decimal? hours = null, DateOnly? date = null, Guid? timePeriodId = null);
    Task<TimePeriod> GetValidTimePeriodForDateAsync(DateOnly date, PeriodType? preferredType = null);
    Task<bool> CanUserCreateEntryInPeriodAsync(Guid userId, Guid timePeriodId);
    Task<decimal> GetTotalHoursForUserInPeriodAsync(Guid userId, Guid timePeriodId);
    Task<bool> WouldExceedDailyLimitAsync(Guid userId, DateOnly date, decimal additionalHours, Guid? excludeTimeEntryId = null);
    Task<bool> WouldExceedPeriodLimitAsync(Guid userId, Guid timePeriodId, decimal additionalHours, Guid? excludeTimeEntryId = null);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public List<string> Errors { get; set; } = new();
    public TimePeriod? SuggestedTimePeriod { get; set; }

    public static ValidationResult Success(TimePeriod? suggestedTimePeriod = null) => new()
    {
        IsValid = true,
        SuggestedTimePeriod = suggestedTimePeriod
    };

    public static ValidationResult Failure(params string[] errors) => new()
    {
        IsValid = false,
        Errors = errors.ToList()
    };

    public static ValidationResult Failure(IEnumerable<string> errors) => new()
    {
        IsValid = false,
        Errors = errors.ToList()
    };
}