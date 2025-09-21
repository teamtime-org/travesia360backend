using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class ServiceType : BaseCatalog, IAggregateRoot
{
    // Navigation properties
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    // Constructors
    private ServiceType() { } // EF Core constructor

    public ServiceType(string code, string name, string? description = null, int displayOrder = 0)
        : base(code, name, description, displayOrder)
    {
    }

    // Factory methods for common service types
    public static ServiceType CreateNuevo() => new("NUEVO", "Nuevo", "Servicio completamente nuevo", 1);
    public static ServiceType CreateExistente() => new("EXISTENTE", "Existente", "Servicio ya existente", 2);
    public static ServiceType CreateRenovacion() => new("RENOVACION", "Renovación", "Renovación de servicio existente", 3);
    public static ServiceType CreateAmpliacion() => new("AMPLIACION", "Ampliación", "Ampliación de servicio existente", 4);
}