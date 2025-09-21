using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class RiskType : BaseCatalog, IAggregateRoot
{
    // Navigation properties
    public virtual ICollection<ProjectRisk> ProjectRisks { get; set; } = new List<ProjectRisk>();

    // Constructors
    private RiskType() { } // EF Core constructor

    public RiskType(string code, string name, string? description = null, int displayOrder = 0)
        : base(code, name, description, displayOrder)
    {
    }

    // Factory methods for common risk types
    public static RiskType CreateOperativo() => new("OPERATIVO", "Operativo", "Riesgos operativos", 1);
    public static RiskType CreateDinero() => new("DINERO", "Dinero", "Riesgos financieros", 2);
    public static RiskType CreateTiempo() => new("TIEMPO", "Tiempo", "Riesgos de tiempo/cronograma", 3);
}