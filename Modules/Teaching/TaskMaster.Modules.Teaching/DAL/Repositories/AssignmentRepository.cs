using Microsoft.EntityFrameworkCore;
using TaskMaster.Modules.Teaching.Entities;
using TaskMaster.Modules.Teaching.Repositories;

namespace TaskMaster.Modules.Teaching.DAL.Repositories;

internal sealed class AssignmentRepository(TeachingDbContext context) : IAssignmentRepository
{
    public async Task AddAssignmentAsync(Assignment assignment, CancellationToken cancellationToken)
    {
        await context.Assignments.AddAsync(assignment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<Assignment?> GetAssignmentAsync(Guid id, CancellationToken cancellationToken)
        => context.Assignments.SingleOrDefaultAsync(a => a.Id == id, cancellationToken);

    public async Task AddAssignmentExerciseAsync(AssignmentExercise assignmentExercise, CancellationToken cancellationToken)
    {
        await context.AssignmentExercises.AddAsync(assignmentExercise, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddClassAssignmentAsync(ClassAssignment classAssignment, CancellationToken cancellationToken)
    {
        await context.ClassAssignments.AddAsync(classAssignment, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<ClassAssignment?> GetClassAssignmentAsync(Guid id, CancellationToken cancellationToken) 
        => context.ClassAssignments.SingleOrDefaultAsync(ca => ca.Id == id, cancellationToken);
}