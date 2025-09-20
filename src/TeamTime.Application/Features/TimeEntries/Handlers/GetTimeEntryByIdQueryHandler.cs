using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.TimeEntries.Queries;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.TimeEntries.Handlers;

public class GetTimeEntryByIdQueryHandler : IQueryHandler<GetTimeEntryByIdQuery, Result<TimeEntryDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<TimeEntry, TimeEntryDto> _mapper;

    public GetTimeEntryByIdQueryHandler(
        IUnitOfWork unitOfWork,
        IMapper<TimeEntry, TimeEntryDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<TimeEntryDto>> HandleAsync(GetTimeEntryByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var timeEntry = await _unitOfWork.TimeEntries.GetByIdAsync(query.Id, cancellationToken);
            if (timeEntry == null)
            {
                return Result<TimeEntryDto>.Failure("Time entry not found");
            }

            var timeEntryDto = _mapper.Map(timeEntry);
            return Result<TimeEntryDto>.Success(timeEntryDto);
        }
        catch (Exception ex)
        {
            return Result<TimeEntryDto>.Failure($"Error retrieving time entry: {ex.Message}");
        }
    }
}