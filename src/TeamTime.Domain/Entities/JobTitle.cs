using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class JobTitle : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string Description { get; private set; } = string.Empty;
    public string Department { get; private set; } = string.Empty;
    public int Level { get; private set; }
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();

    // Constructors
    private JobTitle() { } // EF Core constructor

    public JobTitle(string name, string description, string department, int level = 1)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        Department = department ?? string.Empty;
        Level = level;
        IsActive = true;
    }

    // Domain methods
    public void Update(string name, string description, string department, int level)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description ?? string.Empty;
        Department = department ?? string.Empty;
        Level = level;
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

    // Predefined job titles
    public static class Titles
    {
        public const string ARQUITECTO = "Arquitecto";
        public const string GERENTE_VENTAS = "Gerente de Ventas";
        public const string LIDER_VENTAS = "Líder de Ventas";
        public const string EJECUTIVO_VENTAS = "Ejecutivo de Ventas";
        public const string GERENTE_DISENO_SOLUCIONES = "Gerente de Diseño de Soluciones";
        public const string COORDINADOR_DISENO_SOLUCIONES = "Coordinador de Diseño de Soluciones";
        public const string GERENTE_LICITACIONES = "Gerente de Licitaciones";
        public const string EJECUTIVO_LICITACIONES = "Ejecutivo de Licitaciones";
        public const string COORDINADOR_PM = "Coordinador PM";
        public const string PROJECT_MANAGER = "PM";
        public const string DESARROLLADOR_SENIOR = "Desarrollador Senior";
        public const string DESARROLLADOR = "Desarrollador";
        public const string ANALISTA = "Analista";
        public const string DIRECTOR_AREA = "Director de Área";
    }
}