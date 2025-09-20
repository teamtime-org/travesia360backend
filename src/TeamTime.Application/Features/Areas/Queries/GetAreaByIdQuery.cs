using TeamTime.Application.Common;
using TeamTime.Application.DTOs;
using TeamTime.Common.Results;

namespace TeamTime.Application.Features.Areas.Queries;

public class GetAreaByIdQuery : IQuery<Result<AreaDto>>
{
    public Guid Id { get; set; }
}