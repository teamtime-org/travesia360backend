using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class ContractType : BaseCatalog, IAggregateRoot
{
    // Navigation properties
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    // Constructors
    private ContractType() { } // EF Core constructor

    public ContractType(string code, string name, string? description = null, int displayOrder = 0)
        : base(code, name, description, displayOrder)
    {
    }

    // Factory methods for common contract types
    public static ContractType CreateLicitacion() => new("LICITACION", "Licitación", "Proceso de licitación pública", 1);
    public static ContractType CreateBAU() => new("BAU", "BAU", "Business as Usual", 2);
    public static ContractType CreateCotizacion() => new("COTIZACION", "Cotización", "Proceso de cotización", 3);
    public static ContractType CreateAdjudicacionDirecta() => new("ADJUDICACION_DIRECTA", "Adjudicación Directa", "Adjudicación directa", 4);
    public static ContractType CreateEstudioMercado() => new("ESTUDIO_MERCADO", "Estudio de Mercado", "Estudio de mercado", 5);
    public static ContractType CreateOperacionActual() => new("OPERACION_ACTUAL", "Operación Actual", "Operación actual", 6);
    public static ContractType CreatePrebases() => new("PREBASES", "Prebases", "Fase de prebases", 7);
    public static ContractType CreateAnalisis() => new("ANALISIS", "Análisis", "Análisis de proyecto", 8);
    public static ContractType CreateApoyoTecnico() => new("APOYO_TECNICO", "Apoyo Técnico", "Apoyo técnico", 9);
    public static ContractType CreateInvitacionTres() => new("INVITACION_3", "Invitación a cuando menos 3", "Invitación restringida", 10);
}