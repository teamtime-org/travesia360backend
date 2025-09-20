namespace TeamTime.Application.DTOs;

public class AreaDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int UserCount { get; set; }
    public int ProjectCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}