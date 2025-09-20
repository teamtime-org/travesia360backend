using TeamTime.Application.Common;
using TeamTime.Application.Features.Areas.Commands;
using TeamTime.Common.Results;
using TeamTime.Domain.Interfaces;

namespace TeamTime.Application.Features.Areas.Handlers;

public class DeleteAreaCommandHandler : ICommandHandler<DeleteAreaCommand, Result>
{
    private readonly IUnitOfWork _unitOfWork;

    public DeleteAreaCommandHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    public async Task<Result> HandleAsync(DeleteAreaCommand command, CancellationToken cancellationToken = default)
    {
        try
        {
            // Get existing area
            var area = await _unitOfWork.Areas.GetByIdAsync(command.Id, cancellationToken);
            if (area == null)
            {
                return Result.Failure("Area not found");
            }

            // Check if area has associated users or projects
            var areaWithUsers = await _unitOfWork.Areas.GetWithUsersAsync(command.Id, cancellationToken);
            var areaWithProjects = await _unitOfWork.Areas.GetWithProjectsAsync(command.Id, cancellationToken);

            if (areaWithUsers?.Users?.Any() == true || areaWithProjects?.Projects?.Any() == true)
            {
                // Soft delete - deactivate instead of hard delete
                area.Deactivate();
                await _unitOfWork.Areas.UpdateAsync(area, cancellationToken);
            }
            else
            {
                // Hard delete if no associations
                await _unitOfWork.Areas.DeleteAsync(area, cancellationToken);
            }

            await _unitOfWork.SaveChangesAsync(cancellationToken);
            return Result.Success();
        }
        catch (Exception ex)
        {
            return Result.Failure($"Error deleting area: {ex.Message}");
        }
    }
}