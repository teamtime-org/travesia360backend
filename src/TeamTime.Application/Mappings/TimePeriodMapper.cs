using TeamTime.Application.DTOs;
using TeamTime.Domain.Entities;

namespace TeamTime.Application.Mappings;

public class TimePeriodMapper : IMapper<TimePeriod, TimePeriodDto>
{
    public TimePeriodDto Map(TimePeriod entity)
    {
        return new TimePeriodDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            Type = entity.Type,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            ReferenceHours = entity.ReferenceHours,
            IsActive = entity.IsActive,
            IsCurrent = entity.IsCurrent,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
            WorkingDays = entity.GetWorkingDays(),
            DailyReferenceHours = entity.GetDailyReferenceHours(),
            WeeklyReferenceHours = entity.GetWeeklyReferenceHours(),
            PeriodIdentifier = entity.GetPeriodIdentifier(),
            IsCurrentPeriod = entity.IsCurrentPeriod()
        };
    }

    public TimePeriod Map(TimePeriodDto dto)
    {
        return new TimePeriod(
            dto.Name,
            dto.Description,
            dto.Type,
            dto.StartDate,
            dto.EndDate,
            dto.ReferenceHours);
    }

    public IEnumerable<TimePeriodDto> MapList(IEnumerable<TimePeriod> entities)
    {
        return entities.Select(Map);
    }

    public TimePeriodSummaryDto MapToSummary(TimePeriod entity)
    {
        return new TimePeriodSummaryDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Type = entity.Type,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            ReferenceHours = entity.ReferenceHours,
            IsActive = entity.IsActive,
            IsCurrent = entity.IsCurrent,
            PeriodIdentifier = entity.GetPeriodIdentifier(),
            IsCurrentPeriod = entity.IsCurrentPeriod()
        };
    }

    public TimePeriod MapFromCreate(CreateTimePeriodDto dto)
    {
        return new TimePeriod(
            dto.Name,
            dto.Description,
            dto.Type,
            dto.StartDate,
            dto.EndDate,
            dto.ReferenceHours);
    }

    public void MapUpdate(UpdateTimePeriodDto dto, TimePeriod entity)
    {
        entity.UpdatePeriod(
            dto.Name,
            dto.Description,
            dto.StartDate,
            dto.EndDate,
            dto.ReferenceHours);
    }
}