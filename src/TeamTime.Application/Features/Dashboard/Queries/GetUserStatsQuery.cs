using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Dashboard.Queries;

public class GetUserStatsQuery : IQuery<Result<UserStatsDto>>
{
    public Guid UserId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public bool IncludeProjectBreakdown { get; set; } = true;
    public bool IncludeComparisonPeriod { get; set; } = true;
}