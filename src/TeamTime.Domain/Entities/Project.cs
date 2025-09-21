using TeamTime.Domain.Common;
using TeamTime.Domain.Enums;

namespace TeamTime.Domain.Entities;

public class Project : BaseEntity, IAggregateRoot
{
    // Basic Information (Legacy + Import System)
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }

    // Legacy system fields
    public Guid? AreaId { get; private set; }
    public ProjectStatus Status { get; private set; } = ProjectStatus.ACTIVE;
    public Priority Priority { get; private set; } = Priority.MEDIUM;
    public DateOnly? StartDate { get; private set; }
    public DateOnly? EndDate { get; private set; }
    public decimal? EstimatedHours { get; private set; }
    public bool IsGeneral { get; private set; } = false;
    public bool IsActive { get; private set; } = true;

    // Import system fields (from Project)
    public string? ProjectCode { get; private set; }
    public Guid? ClientId { get; private set; }
    public Guid? SegmentId { get; private set; }
    public Guid? ProjectStageId { get; private set; }
    public Guid? ServiceTypeId { get; private set; }
    public Guid? ContractTypeId { get; private set; }
    public Guid? BusinessLineId { get; private set; }

    // Financial Information (Import system)
    public decimal? ContractValue { get; private set; }
    public decimal? ActualHours { get; private set; }
    public decimal? Progress { get; private set; } // Percentage 0-100

    // Extended Dates (Import system)
    public DateTime? PlannedEndDate { get; private set; }
    public DateTime? ActualEndDate { get; private set; }
    public DateTime? ContractDate { get; private set; }

    // Additional Control fields
    public bool IsConfidential { get; private set; } = false;
    public string? Notes { get; private set; }
    public string? Observations { get; private set; }

    // Import tracking
    public string? ExcelFileName { get; private set; }
    public int? ExcelRowNumber { get; private set; }
    public DateTime? LastImportDate { get; private set; }
    public bool IsImported { get; private set; } = false;

    // Navigation properties
    // Legacy system
    public virtual Area? Area { get; set; }
    public virtual ICollection<ProjectAssignment> Assignments { get; set; } = new List<ProjectAssignment>();
    public virtual ICollection<TeamTimeTask> Tasks { get; set; } = new List<TeamTimeTask>();
    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();

    // Import system relationships
    public virtual Client? Client { get; set; }
    public virtual Segment? Segment { get; set; }
    public virtual ProjectStage? ProjectStage { get; set; }
    public virtual ServiceType? ServiceType { get; set; }
    public virtual ContractType? ContractType { get; set; }
    public virtual BusinessLine? BusinessLine { get; set; }

    // Collections for import system
    public virtual ICollection<ProjectRisk> ProjectRisks { get; set; } = new List<ProjectRisk>();
    public virtual ICollection<ProjectStageHistory> ProjectStageHistory { get; set; } = new List<ProjectStageHistory>();

    // Constructors
    private Project() { } // EF Core constructor

    // Constructor for legacy system
    public Project(string name, Guid areaId, string? description = null, bool isGeneral = false)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        AreaId = areaId;
        Description = description;
        IsGeneral = isGeneral;
        Status = ProjectStatus.ACTIVE;
        Priority = Priority.MEDIUM;
        IsActive = true;
        IsImported = false;
    }

    // Factory method for import system
    public static Project CreateFromImport(
        string projectCode,
        string projectName,
        Guid clientId,
        Guid segmentId,
        Guid projectStageId,
        Guid serviceTypeId,
        Guid contractTypeId,
        Guid businessLineId,
        string? description = null)
    {
        return new Project
        {
            ProjectCode = projectCode?.Trim().ToUpperInvariant() ?? throw new ArgumentNullException(nameof(projectCode)),
            Name = projectName?.Trim() ?? throw new ArgumentNullException(nameof(projectName)),
            Description = description?.Trim(),
            ClientId = clientId,
            SegmentId = segmentId,
            ProjectStageId = projectStageId,
            ServiceTypeId = serviceTypeId,
            ContractTypeId = contractTypeId,
            BusinessLineId = businessLineId,
            IsActive = true,
            IsImported = true,
            Status = ProjectStatus.ACTIVE,
            Priority = Priority.MEDIUM
        };
    }

    // Domain methods
    public void UpdateDetails(string name, string? description = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateDates(DateOnly? startDate, DateOnly? endDate)
    {
        if (startDate.HasValue && endDate.HasValue && startDate > endDate)
            throw new ArgumentException("Start date cannot be later than end date");

        StartDate = startDate;
        EndDate = endDate;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateEstimatedHours(decimal? estimatedHours)
    {
        if (estimatedHours.HasValue && estimatedHours <= 0)
            throw new ArgumentException("Estimated hours must be positive");

        EstimatedHours = estimatedHours;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateStatus(ProjectStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdatePriority(Priority priority)
    {
        Priority = priority;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Complete()
    {
        Status = ProjectStatus.COMPLETED;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel()
    {
        Status = ProjectStatus.CANCELLED;
        UpdatedAt = DateTime.UtcNow;
    }

    public void PutOnHold()
    {
        Status = ProjectStatus.ON_HOLD;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsOverdue => EndDate.HasValue && EndDate < DateOnly.FromDateTime(DateTime.UtcNow) && Status != ProjectStatus.COMPLETED;

    public bool IsInProgress => Status == ProjectStatus.ACTIVE;

    // Import system domain methods (from Project)
    public void UpdateImportInfo(
        Guid? clientId = null,
        Guid? segmentId = null,
        Guid? projectStageId = null,
        Guid? serviceTypeId = null,
        Guid? contractTypeId = null,
        Guid? businessLineId = null)
    {
        if (clientId.HasValue) ClientId = clientId.Value;
        if (segmentId.HasValue) SegmentId = segmentId.Value;
        if (projectStageId.HasValue) ProjectStageId = projectStageId.Value;
        if (serviceTypeId.HasValue) ServiceTypeId = serviceTypeId.Value;
        if (contractTypeId.HasValue) ContractTypeId = contractTypeId.Value;
        if (businessLineId.HasValue) BusinessLineId = businessLineId.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateFinancialInfo(
        decimal? contractValue = null,
        decimal? estimatedHours = null,
        decimal? actualHours = null,
        decimal? progress = null)
    {
        if (contractValue.HasValue) ContractValue = contractValue.Value;
        if (estimatedHours.HasValue) EstimatedHours = estimatedHours.Value;
        if (actualHours.HasValue) ActualHours = actualHours.Value;
        if (progress.HasValue)
        {
            if (progress.Value < 0 || progress.Value > 100)
                throw new ArgumentException("Progress must be between 0 and 100", nameof(progress));
            Progress = progress.Value;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateExtendedDates(
        DateTime? plannedEndDate = null,
        DateTime? actualEndDate = null,
        DateTime? contractDate = null)
    {
        if (plannedEndDate.HasValue) PlannedEndDate = plannedEndDate.Value;
        if (actualEndDate.HasValue) ActualEndDate = actualEndDate.Value;
        if (contractDate.HasValue) ContractDate = contractDate.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string? notes, string? observations = null)
    {
        Notes = notes?.Trim();
        if (observations != null) Observations = observations.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsConfidential(bool isConfidential = true)
    {
        IsConfidential = isConfidential;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateImportMetadata(string? excelFileName = null, int? excelRowNumber = null)
    {
        if (excelFileName != null) ExcelFileName = excelFileName;
        if (excelRowNumber.HasValue) ExcelRowNumber = excelRowNumber.Value;
        LastImportDate = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    // Business logic methods for import system
    public TimeSpan? GetProjectDuration()
    {
        var startDate = StartDate?.ToDateTime(TimeOnly.MinValue) ?? PlannedEndDate ?? ContractDate;
        if (startDate == null) return null;

        var endDate = ActualEndDate ?? PlannedEndDate ?? EndDate?.ToDateTime(TimeOnly.MinValue) ?? DateTime.UtcNow;
        return endDate - startDate.Value;
    }

    public decimal? GetHoursVariance()
    {
        if (EstimatedHours == null || ActualHours == null) return null;
        return ActualHours.Value - EstimatedHours.Value;
    }

    public decimal? GetHoursVariancePercentage()
    {
        if (EstimatedHours == null || ActualHours == null || EstimatedHours == 0) return null;
        return ((ActualHours.Value - EstimatedHours.Value) / EstimatedHours.Value) * 100;
    }

    public bool IsOverdueImport()
    {
        return PlannedEndDate.HasValue &&
               ActualEndDate == null &&
               PlannedEndDate.Value < DateTime.UtcNow.Date;
    }

    public bool IsCompletedImport()
    {
        return ActualEndDate.HasValue || Progress >= 100;
    }

    // Static helper methods from Project
    public static string GenerateProjectCode(string clientCode, string projectName, DateTime? date = null)
    {
        var cleanName = projectName?.Trim().ToUpperInvariant() ?? "PROJECT";
        var year = (date ?? DateTime.UtcNow).Year.ToString()[2..];
        var namePrefix = cleanName.Length >= 3 ? cleanName[..3] : cleanName.PadRight(3, 'X');
        return $"{clientCode}-{namePrefix}-{year}";
    }

    public static string NormalizeProjectName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        return name.Trim()
            .Replace("  ", " ")
            .Replace("\n", " ")
            .Replace("\r", "")
            .Replace("\t", " ");
    }
}