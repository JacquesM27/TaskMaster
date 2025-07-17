using TaskMaster.Abstractions.Queries;
using TaskMaster.Models.Teaching.Assignment;
using TaskMaster.Models.Teaching.School;
using TaskMaster.Modules.Teaching.Entities;
using TaskMaster.Modules.Teaching.Repositories;
using TasMaster.Queries.Exercises.OpenForm;

namespace TaskMaster.Modules.Teaching.Services;

internal sealed class AssignmentService(IAssignmentRepository assignmentRepository, IQueryDispatcher queryDispatcher) : IAssignmentService
{
    public async Task<Guid> AddAssignment(NewAssignmentDto newAssignment, CancellationToken cancellationToken)
    {
        var id = Guid.CreateVersion7();
        var assignment = new Assignment
        {
            Id = id,
            Name = newAssignment.Name,
            Description = newAssignment.Description,
            Exercises = newAssignment.Exercises.Select(exercise => new AssignmentExercise
            {
                Id = Guid.CreateVersion7(),
                ExerciseId = exercise.ExerciseId,
                ExerciseType = exercise.ExerciseType
            }).ToList()
        };
        await assignmentRepository.AddAssignmentAsync(assignment, cancellationToken);
        return id;
    }

    public async Task<AssignmentDetailsDto?> GetAssignmentDetails(Guid assignmentId, CancellationToken cancellationToken)
    {
        var assignment = await assignmentRepository.GetAssignmentAsync(assignmentId, cancellationToken);
        if (assignment is null)
            return null;
        var mailTasks = assignment.Exercises
            .Where(t => t.ExerciseType == "Mail")
            .Select(t => queryDispatcher.QueryAsync(new MailExerciseByIdQuery(t.ExerciseId, cancellationToken)));
        var essayTasks = assignment.Exercises
            .Where(t => t.ExerciseType == "Essay")
            .Select(t => queryDispatcher.QueryAsync(new EssayExerciseByIdQuery(t.ExerciseId, cancellationToken)));
        var summaryTasks = assignment.Exercises
            .Where(t => t.ExerciseType == "Summary")
            .Select(t => queryDispatcher.QueryAsync(new SummaryOfTextExerciseByIdQuery(t.ExerciseId, cancellationToken)));
        
        var mailResults = Task.WhenAll(mailTasks);
        var essayResults = Task.WhenAll(essayTasks);
        var summaryResults = Task.WhenAll(summaryTasks);

        await Task.WhenAll(mailResults, essayResults, summaryResults);
        
        var result = new AssignmentDetailsDto
        {
            Id = assignment.Id,
            Description = assignment.Description,
            Name = assignment.Name,
            MailExercises = mailResults.Result.Where(m => m != null).Select(m => m!).ToList(),
            EssayExercises = essayResults.Result.Where(e => e != null).Select(e => e!).ToList(),
            SummaryOfTextExercises = summaryResults.Result.Where(s => s != null).Select(s => s!).ToList()
        };
        return result;
    }

    public async Task<Guid> AddClassAssignment(NewClassAssignmentDto newClassAssignment, CancellationToken cancellationToken)
    {
        var id = Guid.CreateVersion7();
        var classAssignment = new ClassAssignment
        {
            Id = id,
            AssignmentId = newClassAssignment.AssignmentId,
            Password = newClassAssignment.Password,
            DueDate = newClassAssignment.DueDate,
            TeachingClassId = newClassAssignment.TeachingClassId
        };
        await assignmentRepository.AddClassAssignmentAsync(classAssignment, cancellationToken);
        return id;
    }

    public async Task<Guid> AddClassAssignmentWithoutAssignment(NewClassAssignmentWithoutAssignmentDto newClassAssignment,
        CancellationToken cancellationToken)
    {
        var newAssignmentId = Guid.CreateVersion7();
        var newAssignment = new Assignment()
        {
            Id = newAssignmentId,
            Description = newClassAssignment.Description,
            Exercises = newClassAssignment.Exercises.Select(x => new AssignmentExercise()
            {
                Id = Guid.CreateVersion7(),
                ExerciseId = x.ExerciseId,
                ExerciseType = x.ExerciseType
            })
        };
        await assignmentRepository.AddAssignmentAsync(newAssignment, cancellationToken);

        var classAssignmentId = Guid.CreateVersion7();
        var classAssignment = new ClassAssignment()
        {
            Id = classAssignmentId,
            AssignmentId = newAssignmentId,
            TeachingClassId = newClassAssignment.TeachingClassId,
            Password = newClassAssignment.Password,
            DueDate = newClassAssignment.DueDate,
        };
        await assignmentRepository.AddClassAssignmentAsync(classAssignment, cancellationToken);

        return classAssignmentId;
    }

    public async Task<ClassAssignmentDetailsDto?> GetClassAssignmentDetails(Guid classAssignmentId, CancellationToken cancellationToken)
    {
        var classAssignment = await assignmentRepository.GetClassAssignmentAsync(classAssignmentId, cancellationToken);
        if (classAssignment is null)
            return null;

        var assignmentDetails = await GetAssignmentDetails(classAssignment.AssignmentId, cancellationToken);

        var result = new ClassAssignmentDetailsDto
        {
            Id = classAssignment.Id,
            DueDate = classAssignment.DueDate,
            Assignment = assignmentDetails
        };
        return result;
    }
}