using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Dashboard.Queries;

public class GetTimeDistributionQuery : IQuery<Result<TimeDistributionSummaryDto>>
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? UserId { get; set; }
    public Guid? AreaId { get; set; }
    public string GroupBy { get; set; } = "project"; // "project", "area", "both"
    public bool IncludeSubItems { get; set; } = true;
    public int MaxItems { get; set; } = 10;
}