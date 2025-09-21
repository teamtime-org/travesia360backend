namespace TeamTime.Domain.Common;

public abstract class BaseCatalog : BaseEntity
{
    public string Code { get; protected set; } = string.Empty;
    public string Name { get; protected set; } = string.Empty;
    public string? Description { get; protected set; }
    public bool IsActive { get; protected set; } = true;
    public int DisplayOrder { get; protected set; } = 0;

    protected BaseCatalog() { }

    protected BaseCatalog(string code, string name, string? description = null, int displayOrder = 0)
    {
        Code = code ?? throw new ArgumentNullException(nameof(code));
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        DisplayOrder = displayOrder;
        IsActive = true;
    }

    public virtual void Update(string name, string? description = null, int? displayOrder = null)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Description = description;
        if (displayOrder.HasValue)
        {
            DisplayOrder = displayOrder.Value;
        }
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void Activate()
    {
        IsActive = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public virtual void Deactivate()
    {
        IsActive = false;
        UpdatedAt = DateTime.UtcNow;
    }
}