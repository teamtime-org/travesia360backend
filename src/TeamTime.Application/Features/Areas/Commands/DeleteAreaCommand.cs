using TeamTime.Application.Common;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Areas.Commands;

public class DeleteAreaCommand : ICommand<Result>
{
    public Guid Id { get; set; }
}