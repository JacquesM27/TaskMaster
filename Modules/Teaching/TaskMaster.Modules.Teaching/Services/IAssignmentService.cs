using TaskMaster.Models.Teaching.Assignment;
using TaskMaster.Models.Teaching.School;

namespace TaskMaster.Modules.Teaching.Services;

public interface IAssignmentService
{
    Task<Guid> AddAssignment(NewAssignmentDto newAssignment, CancellationToken cancellationToken);
    Task<AssignmentDetailsDto?> GetAssignmentDetails(Guid assignmentId, CancellationToken cancellationToken);
    Task<Guid> AddClassAssignment(NewClassAssignmentDto newClassAssignment, CancellationToken cancellationToken);
    Task<Guid> AddClassAssignmentWithoutAssignment(NewClassAssignmentWithoutAssignmentDto newClassAssignment, CancellationToken cancellationToken);
    Task<ClassAssignmentDetailsDto?> GetClassAssignmentDetails(Guid classAssignmentId, CancellationToken cancellationToken);
}