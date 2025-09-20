using TeamTime.Domain.Entities;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.Services;

public interface ITimePeriodService
{
    Task<TimePeriod> GetCurrentPeriodAsync(PeriodType periodType);
    Task<TimePeriod> GetOrCreateCurrentPeriodAsync(PeriodType periodType, decimal? referenceHours = null);
    Task<TimePeriod> CreatePeriodAsync(PeriodType periodType, DateTime startDate, decimal referenceHours);
    Task<IEnumerable<TimePeriod>> GeneratePeriodsForYearAsync(int year, PeriodType periodType, decimal referenceHours);
    Task<TimePeriod?> FindPeriodForDateAsync(DateTime date, PeriodType? periodType = null);
    Task<IEnumerable<TimePeriod>> GetActivePeriodsAsync();
    Task<bool> SetCurrentPeriodAsync(Guid periodId);
    Task<bool> ValidateTimePeriodForDateAsync(DateTime date, Guid timePeriodId);
    Task<IEnumerable<TimePeriod>> GetUserAccessiblePeriodsAsync(Guid userId);
}