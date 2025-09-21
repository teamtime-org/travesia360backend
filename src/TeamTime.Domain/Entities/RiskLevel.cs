using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class RiskLevel : BaseCatalog, IAggregateRoot
{
    public string? Color { get; private set; }

    // Navigation properties
    public virtual ICollection<ProjectRisk> ProjectRisks { get; set; } = new List<ProjectRisk>();

    // Constructors
    private RiskLevel() { } // EF Core constructor

    public RiskLevel(string code, string name, string? description = null, string? color = null, int displayOrder = 0)
        : base(code, name, description, displayOrder)
    {
        Color = color;
    }

    public void UpdateColor(string? color)
    {
        Color = color;
        UpdatedAt = DateTime.UtcNow;
    }

    // Factory methods for common risk levels
    public static RiskLevel CreateBajo() => new("BAJO", "Bajo", "Riesgo bajo", "#28a745", 1);
    public static RiskLevel CreateMedio() => new("MEDIO", "Medio", "Riesgo medio", "#ffc107", 2);
    public static RiskLevel CreateAlto() => new("ALTO", "Alto", "Riesgo alto", "#dc3545", 3);
}