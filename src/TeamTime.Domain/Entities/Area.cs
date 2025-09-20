using TeamTime.Domain.Common;

namespace TeamTime.Domain.Entities;

public class Area : BaseEntity, IAggregateRoot
{
    public string Name { get; private set; } = string.Empty;
    public string? Description { get; private set; }
    public string Color { get; private set; } = "#2563EB";
    public bool IsActive { get; private set; } = true;

    // Navigation properties
    public virtual ICollection<User> Users { get; set; } = new List<User>();
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();

    // Constructors
    private Area() { } // EF Core constructor

    public Area(string name, string? description = null, string color = "#2563EB")
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        Color = color;
        IsActive = true;
    }

    // Domain methods
    public void UpdateDetails(string name, string? description = null, string? color = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        if (!string.IsNullOrEmpty(color))
            Color = color;
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
}