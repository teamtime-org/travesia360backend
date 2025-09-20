using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Dashboard.Queries;

public class GetProjectStatsQuery : IQuery<Result<ProjectStatsDto>>
{
    public Guid ProjectId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public bool IncludeTeamMembers { get; set; } = true;
    public bool IncludeTaskBreakdown { get; set; } = true;
}