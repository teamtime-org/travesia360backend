using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Dashboard.Queries;
using TeamTime.Common.Results;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Dashboard.Handlers;

public class GetTimeDistributionQueryHandler : IQueryHandler<GetTimeDistributionQuery, Result<TimeDistributionSummaryDto>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetTimeDistributionQueryHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result<TimeDistributionSummaryDto>> HandleAsync(GetTimeDistributionQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var dateFrom = DateOnly.FromDateTime(query.DateFrom ?? DateTime.Now.Date.AddDays(-30));
            var dateTo = DateOnly.FromDateTime(query.DateTo ?? DateTime.Now.Date);

            // Get time entries for the period
            var timeEntries = await _unitOfWork.TimeEntries.GetByDateRangeAsync(dateFrom, dateTo, cancellationToken);
            var totalHours = timeEntries.Sum(te => te.Hours);

            if (totalHours == 0)
            {
                return Result<TimeDistributionSummaryDto>.Success(new TimeDistributionSummaryDto
                {
                    TotalHours = 0,
                    ByProject = new List<TimeDistributionDto>(),
                    ByArea = new List<TimeDistributionDto>(),
                    DateFrom = dateFrom.ToDateTime(TimeOnly.MinValue),
                    DateTo = dateTo.ToDateTime(TimeOnly.MinValue)
                });
            }

            var byProject = new List<TimeDistributionDto>();
            var byArea = new List<TimeDistributionDto>();

            // Get distribution by project
            if (query.GroupBy == "project" || query.GroupBy == "both")
            {
                byProject = timeEntries
                    .GroupBy(te => te.ProjectId)
                    .Select(g => new TimeDistributionDto
                    {
                        Id = g.Key,
                        Name = g.First().Project?.Name ?? "Unknown Project",
                        Type = "Project",
                        Hours = g.Sum(te => te.Hours),
                        Percentage = (g.Sum(te => te.Hours) / totalHours) * 100,
                        Color = GenerateColor(g.Key),
                        SubItems = new List<TimeDistributionDto>()
                    })
                    .OrderByDescending(p => p.Hours)
                    .Take(query.MaxItems)
                    .ToList();
            }

            // Get distribution by area (through project)
            if (query.GroupBy == "area" || query.GroupBy == "both")
            {
                byArea = timeEntries
                    .Where(te => te.Project?.Area != null)
                    .GroupBy(te => te.Project.AreaId)
                    .Select(g => new TimeDistributionDto
                    {
                        Id = g.Key ?? Guid.Empty,
                        Name = g.First().Project?.Area?.Name ?? "Unknown Area",
                        Type = "Area",
                        Hours = g.Sum(te => te.Hours),
                        Percentage = (g.Sum(te => te.Hours) / totalHours) * 100,
                        Color = GenerateColor(g.Key ?? Guid.Empty),
                        SubItems = new List<TimeDistributionDto>()
                    })
                    .OrderByDescending(a => a.Hours)
                    .Take(query.MaxItems)
                    .ToList();
            }

            var result = new TimeDistributionSummaryDto
            {
                TotalHours = totalHours,
                ByProject = byProject,
                ByArea = byArea,
                DateFrom = dateFrom.ToDateTime(TimeOnly.MinValue),
                DateTo = dateTo.ToDateTime(TimeOnly.MinValue)
            };

            return Result<TimeDistributionSummaryDto>.Success(result);
        }
        catch (Exception ex)
        {
            return Result<TimeDistributionSummaryDto>.Failure($"Error retrieving time distribution: {ex.Message}");
        }
    }

    private string GenerateColor(Guid id)
    {
        // Generate a consistent color based on the ID
        var hash = id.GetHashCode();
        var r = (hash & 0xFF0000) >> 16;
        var g = (hash & 0x00FF00) >> 8;
        var b = hash & 0x0000FF;

        // Ensure colors are not too dark
        r = Math.Max(r, 100);
        g = Math.Max(g, 100);
        b = Math.Max(b, 100);

        return $"#{r:X2}{g:X2}{b:X2}";
    }
}