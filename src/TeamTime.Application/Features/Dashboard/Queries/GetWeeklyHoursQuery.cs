using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Dashboard.Queries;

public class GetWeeklyHoursQuery : IQuery<Result<WeeklyHoursSummaryDto>>
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? UserId { get; set; }
    public Guid? ProjectId { get; set; }
    public Guid? AreaId { get; set; }
    public bool IncludeDailyBreakdown { get; set; } = true;
    public bool IncludeProjectBreakdown { get; set; } = false;
    public decimal WeeklyTarget { get; set; } = 40;
    public int WeeksCount { get; set; } = 12; // Default to last 12 weeks
}