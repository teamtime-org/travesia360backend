namespace TeamTime.Application.DTOs;

public class CreateAreaDto
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Color { get; set; } = "#2563EB";
}