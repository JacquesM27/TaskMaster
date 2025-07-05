using TaskMaster.Abstractions.Modules;

namespace TaskMaster.Modules.Teaching.Contracts;

public interface ITeachingModuleClient : IModuleClient
{
    Task<AssignmentDto?> GetAssignmentAsync(Guid assignmentId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AssignmentDto>> GetAssignmentsForUserAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<IEnumerable<AssignmentDto>> GetAssignmentsForClassAsync(Guid classId, CancellationToken cancellationToken = default);
    Task<bool> IsUserTeacherInSchoolAsync(Guid userId, Guid schoolId, CancellationToken cancellationToken = default);
}

public sealed record AssignmentDto(
    Guid Id,
    string Title,
    string Description,
    DateTime DueDate,
    Guid CreatedBy,
    Guid ClassId,
    DateTime CreatedAt);

public sealed record TeachingClassDto(
    Guid Id,
    string Name,
    string Description,
    Guid SchoolId,
    DateTime CreatedAt);