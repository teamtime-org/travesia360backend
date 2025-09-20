namespace TeamTime.Application.DTOs;

public class UpdateAreaDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public bool? IsActive { get; set; }
}