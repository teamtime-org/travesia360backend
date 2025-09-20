using System.ComponentModel.DataAnnotations;
using TeamTime.Domain.Enums;

namespace TeamTime.Application.DTOs;

public class CreateTimePeriodDto
{
    [Required]
    [StringLength(255, MinimumLength = 1)]
    public string Name { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    [Required]
    public PeriodType Type { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    [Required]
    [Range(0.1, 400.0, ErrorMessage = "Reference hours must be between 0.1 and 400")]
    public decimal ReferenceHours { get; set; }

    public bool IsActive { get; set; } = true;

    public bool IsCurrent { get; set; } = false;
}