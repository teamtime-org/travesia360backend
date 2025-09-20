using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Areas.Queries;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Areas.Handlers;

public class GetAreaByIdQueryHandler : IQueryHandler<GetAreaByIdQuery, Result<AreaDto>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<Area, AreaDto> _mapper;

    public GetAreaByIdQueryHandler(IUnitOfWork unitOfWork, IMapper<Area, AreaDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<AreaDto>> HandleAsync(GetAreaByIdQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            var area = await _unitOfWork.Areas.GetByIdAsync(query.Id, cancellationToken);

            if (area == null)
            {
                return Result<AreaDto>.Failure("Area not found");
            }

            var areaDto = _mapper.Map(area);
            return Result<AreaDto>.Success(areaDto);
        }
        catch (Exception ex)
        {
            return Result<AreaDto>.Failure($"Error retrieving area: {ex.Message}");
        }
    }
}