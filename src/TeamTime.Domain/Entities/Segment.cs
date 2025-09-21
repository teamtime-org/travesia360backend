using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class Segment : BaseCatalog, IAggregateRoot
{
    // Navigation properties
    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    // Constructors
    private Segment() { } // EF Core constructor

    public Segment(string code, string name, string? description = null, int displayOrder = 0)
        : base(code, name, description, displayOrder)
    {
    }

    // Factory methods for common segments
    public static Segment CreateFederal() => new("FEDERAL", "Federal", "Proyectos del gobierno federal", 1);
    public static Segment CreateEstatal() => new("ESTATAL", "Estatal", "Proyectos del gobierno estatal", 2);
    public static Segment CreateMunicipal() => new("MUNICIPAL", "Municipal", "Proyectos del gobierno municipal", 3);
    public static Segment CreateEstatelEstatal() => new("ESTRATEL_ESTATAL", "Estratel-Estatal", "Proyectos Estratel estatales", 4);
    public static Segment CreateEstatelEmpresarial() => new("ESTRATEL_EMPRESARIAL", "Estratel-Empresarial", "Proyectos Estratel empresariales", 5);
}