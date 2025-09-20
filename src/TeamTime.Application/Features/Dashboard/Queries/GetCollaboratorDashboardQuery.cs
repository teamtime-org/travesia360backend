using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Dashboard.Queries;

public class GetCollaboratorDashboardQuery : IQuery<Result<CollaboratorDashboardDto>>
{
    public Guid UserId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int RecentTasksCount { get; set; } = 5;
    public int RecentTimeEntriesCount { get; set; } = 10;
    public decimal WeeklyHoursTarget { get; set; } = 40;
    public bool IncludeWeeklyProgress { get; set; } = true;
    public bool IncludeTimeDistribution { get; set; } = true;
}