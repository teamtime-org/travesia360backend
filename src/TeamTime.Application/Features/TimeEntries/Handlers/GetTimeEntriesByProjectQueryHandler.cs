using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.TimeEntries.Queries;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.TimeEntries.Handlers;

public class GetTimeEntriesByProjectQueryHandler : IQueryHandler<GetTimeEntriesByProjectQuery, Result<IEnumerable<TimeEntryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TimeEntry, TimeEntryDto> _mapper;

    public GetTimeEntriesByProjectQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper<TimeEntry, TimeEntryDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<IEnumerable<TimeEntryDto>>> HandleAsync(GetTimeEntriesByProjectQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var pagedResult = await _unitOfWork.TimeEntries.GetPagedByProjectAsync(
                query.ProjectId,
                query.Page,
                query.PageSize,
                query.StartDate,
                query.EndDate,
                query.IsApproved,
                cancellationToken);

            var timeEntryDtos = _mapper.MapList(pagedResult.Items);
            return Result<IEnumerable<TimeEntryDto>>.Success(timeEntryDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TimeEntryDto>>.Failure($"Error retrieving time entries by project: {ex.Message}");
        }
    }
}