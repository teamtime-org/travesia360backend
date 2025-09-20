using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Areas.Commands;

public class UpdateAreaCommand : ICommand<Result<AreaDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Color { get; set; }
    public bool? IsActive { get; set; }
}