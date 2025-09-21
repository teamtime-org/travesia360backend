using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class BusinessLine : BaseCatalog, IAggregateRoot
{
    // Navigation properties
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    // Constructors
    private BusinessLine() { } // EF Core constructor

    public BusinessLine(string code, string name, string? description = null, int displayOrder = 0)
        : base(code, name, description, displayOrder)
    {
    }

    // Factory methods for common business lines
    public static BusinessLine CreateTelco() => new("TELCO", "TELCO", "Telecomunicaciones", 1);
    public static BusinessLine CreateTI() => new("TI", "TI", "Tecnologías de la Información", 2);
    public static BusinessLine CreateServiciosEsenciales() => new("SERVICIOS_ESENCIALES", "Servicios Esenciales", "Servicios esenciales", 3);
    public static BusinessLine CreateRedesAdministradas() => new("REDES_ADMINISTRADAS", "Redes Administradas", "Redes administradas", 4);
}