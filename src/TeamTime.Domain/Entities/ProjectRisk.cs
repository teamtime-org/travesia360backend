using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class ProjectRisk : BaseEntity, IAggregateRoot
{
    public Guid ProjectId { get; private set; }
    public Guid RiskTypeId { get; private set; }
    public Guid RiskLevelId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string? Mitigation { get; private set; }
    public DateTime IdentifiedDate { get; private set; }
    public DateTime? MitigatedDate { get; private set; }
    public bool IsActive { get; private set; } = true;
    public decimal? Impact { get; private set; } // 1-5 scale
    public decimal? Probability { get; private set; } // 1-5 scale

    // Navigation properties
    public virtual Project Project { get; set; } = null!;
    public virtual RiskType RiskType { get; set; } = null!;
    public virtual RiskLevel RiskLevel { get; set; } = null!;

    // Constructors
    private ProjectRisk() { } // EF Core constructor

    public ProjectRisk(
        Guid projectId,
        Guid riskTypeId,
        Guid riskLevelId,
        string title,
        string? description = null,
        DateTime? identifiedDate = null)
    {
        ProjectId = projectId;
        RiskTypeId = riskTypeId;
        RiskLevelId = riskLevelId;
        Title = title?.Trim() ?? throw new ArgumentNullException(nameof(title));
        Description = description?.Trim();
        IdentifiedDate = identifiedDate ?? DateTime.UtcNow;
        IsActive = true;
    }

    // Domain methods
    public void UpdateInfo(string title, string? description = null)
    {
        Title = title?.Trim() ?? throw new ArgumentNullException(nameof(title));
        Description = description?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateRiskLevel(Guid riskLevelId)
    {
        RiskLevelId = riskLevelId;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateMitigation(string? mitigation)
    {
        Mitigation = mitigation?.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateImpactAndProbability(decimal? impact, decimal? probability)
    {
        if (impact.HasValue)
        {
            if (impact.Value < 1 || impact.Value > 5)
                throw new ArgumentException("Impact must be between 1 and 5", nameof(impact));
            Impact = impact.Value;
        }

        if (probability.HasValue)
        {
            if (probability.Value < 1 || probability.Value > 5)
                throw new ArgumentException("Probability must be between 1 and 5", nameof(probability));
            Probability = probability.Value;
        }

        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsMitigated(DateTime? mitigatedDate = null)
    {
        MitigatedDate = mitigatedDate ?? DateTime.UtcNow;
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

    // Business logic methods
    public decimal? GetRiskScore()
    {
        if (Impact == null || Probability == null) return null;
        return Impact.Value * Probability.Value;
    }

    public bool IsMitigated()
    {
        return MitigatedDate.HasValue;
    }

    public TimeSpan? GetMitigationTime()
    {
        if (!MitigatedDate.HasValue) return null;
        return MitigatedDate.Value - IdentifiedDate;
    }
}