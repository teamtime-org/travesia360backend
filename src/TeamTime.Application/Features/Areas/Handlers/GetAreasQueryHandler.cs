using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Application.Features.Areas.Queries;
using TeamTime.Application.Mappings;
using TeamTime.Common.Results;
using TeamTime.Domain.Entities;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Areas.Handlers;

public class GetAreasQueryHandler : IQueryHandler<GetAreasQuery, Result<PagedResult<AreaDto>>>
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper<Area, AreaDto> _mapper;

    public GetAreasQueryHandler(IUnitOfWork unitOfWork, IMapper<Area, AreaDto> mapper)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Result<PagedResult<AreaDto>>> HandleAsync(GetAreasQuery query, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get all areas
            var allAreas = await _unitOfWork.Areas.GetAllAsync(cancellationToken);

            // Apply filters
            var filteredAreas = allAreas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(query.SearchTerm))
            {
                var searchTerm = query.SearchTerm.ToLower();
                filteredAreas = filteredAreas.Where(a =>
                    a.Name.ToLower().Contains(searchTerm) ||
                    (a.Description != null && a.Description.ToLower().Contains(searchTerm)));
            }

            if (query.IsActive.HasValue)
            {
                filteredAreas = filteredAreas.Where(a => a.IsActive == query.IsActive.Value);
            }

            // Count for pagination
            var totalCount = filteredAreas.Count();

            // Apply pagination
            var pagedAreas = filteredAreas
                .Skip((query.PageNumber - 1) * query.PageSize)
                .Take(query.PageSize)
                .ToList();

            // Map to DTOs
            var areaDtos = pagedAreas.Select(_mapper.Map).ToList();

            // Create paged result
            var pagedResult = PagedResult<AreaDto>.Create(
                areaDtos,
                totalCount,
                query.PageNumber,
                query.PageSize);

            return Result<PagedResult<AreaDto>>.Success(pagedResult);
        }
        catch (Exception ex)
        {
            return Result<PagedResult<AreaDto>>.Failure($"Error retrieving areas: {ex.Message}");
        }
    }
}