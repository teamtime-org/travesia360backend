using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class ProjectStage : BaseCatalog, IAggregateRoot
{
    public string Phase { get; private set; } = string.Empty; // DISEÑO, EJECUCIÓN, CIERRE
    public string? Color { get; private set; }
    public bool IsFinalStage { get; private set; } = false;

    // Navigation properties
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
    public virtual ICollection<ProjectStageHistory> StageHistories { get; set; } = new List<ProjectStageHistory>();

    // Constructors
    private ProjectStage() { } // EF Core constructor

    public ProjectStage(string code, string name, string phase, string? description = null,
        string? color = null, bool isFinalStage = false, int displayOrder = 0)
        : base(code, name, description, displayOrder)
    {
        Phase = phase ?? throw new ArgumentNullException(nameof(phase));
        Color = color;
        IsFinalStage = isFinalStage;
    }

    public void UpdatePhase(string phase, string? color = null, bool? isFinalStage = null)
    {
        Phase = phase ?? throw new ArgumentNullException(nameof(phase));
        if (color != null) Color = color;
        if (isFinalStage.HasValue) IsFinalStage = isFinalStage.Value;
        UpdatedAt = DateTime.UtcNow;
    }

    // Factory methods for common stages
    public static class Stages
    {
        // DISEÑO Phase
        public static ProjectStage CreateAdjudicado() => new("ADJUDICADO", "Adjudicado", "DISEÑO", "Proyecto adjudicado", "#28a745", false, 10);
        public static ProjectStage CreateEnProceso() => new("EN_PROCESO", "En proceso", "DISEÑO", "Proyecto en proceso", "#17a2b8", false, 20);
        public static ProjectStage CreatePerdido() => new("PERDIDO", "Perdido", "DISEÑO", "Proyecto perdido", "#dc3545", true, 30);
        public static ProjectStage CreateDesierto() => new("DESIERTO", "Desierto", "DISEÑO", "Licitación desierta", "#6c757d", true, 40);
        public static ProjectStage CreateSinParticipacion() => new("SIN_PARTICIPACION", "Sin participación", "DISEÑO", "No se participó", "#6c757d", true, 50);
        public static ProjectStage CreateCanceladoDiseño() => new("CANCELADO_DISEÑO", "Cancelado", "DISEÑO", "Proyecto cancelado en diseño", "#dc3545", true, 60);
        public static ProjectStage CreateDetenido() => new("DETENIDO", "Detenido", "DISEÑO", "Proyecto detenido", "#ffc107", false, 70);

        // EJECUCIÓN Phase
        public static ProjectStage CreateEjecucion() => new("EJECUCION", "Ejecución", "EJECUCIÓN", "Proyecto en ejecución", "#007bff", false, 100);
        public static ProjectStage CreateCompletado() => new("COMPLETADO", "Completado", "EJECUCIÓN", "Proyecto completado", "#28a745", false, 110);
        public static ProjectStage CreateGanada() => new("GANADA", "Ganada", "EJECUCIÓN", "Oportunidad ganada", "#28a745", false, 120);
        public static ProjectStage CreatePerdida() => new("PERDIDA", "Perdida", "EJECUCIÓN", "Oportunidad perdida", "#dc3545", true, 130);

        // CIERRE Phase
        public static ProjectStage CreateEntregado() => new("ENTREGADO", "Entregado", "CIERRE", "Proyecto entregado", "#28a745", true, 200);
        public static ProjectStage CreateCanceladoCierre() => new("CANCELADO_CIERRE", "Cancelado", "CIERRE", "Proyecto cancelado", "#dc3545", true, 210);
    }
}