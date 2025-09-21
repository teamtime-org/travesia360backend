using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class ProjectRole : BaseCatalog, IAggregateRoot
{
    // Navigation properties
    public virtual ICollection<ProjectAssignment> ProjectAssignments { get; set; } = new List<ProjectAssignment>();

    // Constructors
    private ProjectRole() { } // EF Core constructor

    public ProjectRole(string code, string name, string? description = null, int displayOrder = 0)
        : base(code, name, description, displayOrder)
    {
    }

    // Factory methods for common project roles
    public static ProjectRole CreateArquitecto() => new("ARQUITECTO", "Arquitecto", "Arquitecto del proyecto", 1);
    public static ProjectRole CreateGerenteVentas() => new("GERENTE_VENTAS", "Gerente Ventas", "Gerente de ventas", 2);
    public static ProjectRole CreateLiderVentas() => new("LIDER_VENTAS", "Líder Ventas", "Líder de ventas", 3);
    public static ProjectRole CreateEjecutivoVentas() => new("EJECUTIVO_VENTAS", "Ejecutivo Ventas", "Ejecutivo de ventas", 4);
    public static ProjectRole CreateGerenteDS() => new("GERENTE_DS", "Gerente DS", "Gerente de diseño", 5);
    public static ProjectRole CreateCoordinadorDS() => new("COORDINADOR_DS", "Coordinador DS", "Coordinador de diseño", 6);
    public static ProjectRole CreateGerenteLicitaciones() => new("GERENTE_LICITACIONES", "Gerente Licitaciones", "Gerente de licitaciones", 7);
    public static ProjectRole CreateEjecutivoLicitaciones() => new("EJECUTIVO_LICITACIONES", "Ejecutivo Licitaciones", "Ejecutivo de licitaciones", 8);
    public static ProjectRole CreateMentor() => new("MENTOR", "Mentor", "Mentor del proyecto", 9);
    public static ProjectRole CreateCoordinador() => new("COORDINADOR", "Coordinador", "Coordinador del proyecto", 10);
    public static ProjectRole CreateDiseñador() => new("DISEÑADOR", "Diseñador", "Diseñador del proyecto", 11);
}