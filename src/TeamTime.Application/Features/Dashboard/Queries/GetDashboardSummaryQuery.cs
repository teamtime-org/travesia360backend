using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Dashboard.Queries;

public class GetDashboardSummaryQuery : IQuery<Result<DashboardSummaryDto>>
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? AreaId { get; set; }
    public bool IncludeInactive { get; set; } = false;
    public int TopProjectsCount { get; set; } = 5;
    public int TopUsersCount { get; set; } = 5;
}