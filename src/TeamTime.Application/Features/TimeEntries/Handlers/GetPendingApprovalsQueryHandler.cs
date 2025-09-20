using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.TimeEntries.Queries;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.TimeEntries.Handlers;

public class GetPendingApprovalsQueryHandler : IQueryHandler<GetPendingApprovalsQuery, Result<IEnumerable<TimeEntryDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TimeEntry, TimeEntryDto> _mapper;

    public GetPendingApprovalsQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper<TimeEntry, TimeEntryDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<IEnumerable<TimeEntryDto>>> HandleAsync(GetPendingApprovalsQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var pagedResult = await _unitOfWork.TimeEntries.GetPagedPendingApprovalsAsync(
                query.Page,
                query.PageSize,
                query.ProjectId,
                query.StartDate,
                query.EndDate,
                cancellationToken);

            var timeEntryDtos = _mapper.MapList(pagedResult.Items);
            return Result<IEnumerable<TimeEntryDto>>.Success(timeEntryDtos);
        }
        catch (Exception ex)
        {
            return Result<IEnumerable<TimeEntryDto>>.Failure($"Error retrieving pending approvals: {ex.Message}");
        }
    }
}