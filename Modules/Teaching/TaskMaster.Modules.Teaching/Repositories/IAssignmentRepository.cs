using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.Repositories;

public interface IAssignmentRepository
{
    Task AddAssignmentAsync(Assignment assignment, CancellationToken cancellationToken);
    Task<Assignment?> GetAssignmentAsync(Guid id, CancellationToken cancellationToken);
    
    Task AddAssignmentExerciseAsync(Assignment assignment, AssignmentExercise exercise, CancellationToken cancellationToken);
    
    Task AddClassAssignmentAsync(ClassAssignment classAssignment, CancellationToken cancellationToken);
    Task<ClassAssignment?> GetClassAssignmentAsync(Guid id, CancellationToken cancellationToken);
}