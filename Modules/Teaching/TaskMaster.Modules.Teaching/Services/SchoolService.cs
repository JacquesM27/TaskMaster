using TaskMaster.Abstractions.Queries;
using TaskMaster.Models.Teaching.School;
using TaskMaster.Modules.Teaching.Entities;
using TaskMaster.Modules.Teaching.Exceptions;
using TaskMaster.Modules.Teaching.Repositories;
using TasMaster.Queries.Accounts;

namespace TaskMaster.Modules.Teaching.Services;

internal sealed class SchoolService(ISchoolRepository repo, IQueryDispatcher queryDispatcher) : ISchoolService
{
    public async Task<Guid> CreateSchoolAsync(NewSchoolDto newSchoolDto, CancellationToken cancellationToken)
    {
        var school = new School
        {
            Id = Guid.CreateVersion7(),
            Name = newSchoolDto.Name,
            SchoolAdmins = []
        };
        
        if (newSchoolDto.AdminsMails.Any())
        {
            var adminsTasks = newSchoolDto.AdminsMails
                .Select(mail => new UserIdByEmailQuery(mail))
                .Select(queryDispatcher.QueryAsync)
                .ToList();

            await foreach (var finishedTask in Task.WhenEach(adminsTasks).WithCancellation(cancellationToken))
            {
                if (!finishedTask.Result.HasValue)
                    continue;

                school.SchoolAdmins.Add(new SchoolAdmin
                {
                    AdminId = finishedTask.Result.Value,
                    School = school,
                    SchoolId = school.Id,
                });
            }
        }

        if (newSchoolDto.TeachersMails.Any())
        {
            var teachersTasks = newSchoolDto.TeachersMails
                .Select(m => new UserIdByEmailQuery(m))
                .Select(queryDispatcher.QueryAsync)
                .ToList();

            await foreach (var finishedTask in Task.WhenEach(teachersTasks).WithCancellation(cancellationToken))
            {
                if (!finishedTask.Result.HasValue)
                    continue;
                
                school.SchoolTeachers.Add(new SchoolTeacher()
                {
                    School = school,
                    SchoolId = school.Id,
                    TeacherId = finishedTask.Result.Value
                });
            }
        }

        await repo.AddSchoolAsync(school, cancellationToken);
        return school.Id;
    }

    public async Task<Guid> CreateTeachingClassAsync(NewTeachingClassDto newTeachingClassDto, CancellationToken cancellationToken)
    {
        var school = await repo.GetSchoolAsync(newTeachingClassDto.SchoolId, cancellationToken)
                     ?? throw new SchoolNotFoundException(newTeachingClassDto.SchoolId);

        var mainTeacherQuery = new UserIdByEmailQuery(newTeachingClassDto.MainTeacherMail);
        var mainTeacherId = await queryDispatcher.QueryAsync(mainTeacherQuery)
                            ?? throw new TeacherNotFoundException(newTeachingClassDto.MainTeacherMail);
        
        var subTeacherIds = new List<Guid>();
        if (newTeachingClassDto.SubTeachersMails.Any())
        {
            var subTeachersTasks = newTeachingClassDto.SubTeachersMails
                .Select(m => new UserIdByEmailQuery(m))
                .Select(queryDispatcher.QueryAsync)
                .ToList();

            await foreach (var finishedTask in Task.WhenEach(subTeachersTasks).WithCancellation(cancellationToken))
            {
                if (!finishedTask.Result.HasValue)
                    continue;
                
                subTeacherIds.Add(finishedTask.Result.Value);
            }
        }
        
        var studentIds = new List<Guid>();
        if (newTeachingClassDto.StudentsMails.Any())
        {
            var studentsTasks = newTeachingClassDto.StudentsMails
                .Select(m => new UserIdByEmailQuery(m))
                .Select(queryDispatcher.QueryAsync)
                .ToList();

            await foreach (var finishedTask in Task.WhenEach(studentsTasks).WithCancellation(cancellationToken))
            {
                if (!finishedTask.Result.HasValue)
                    continue;
                
                studentIds.Add(finishedTask.Result.Value);
            }
        }

        var teachingClass = new TeachingClass()
        {
            Id = Guid.CreateVersion7(),
            Name = newTeachingClassDto.Name,
            Level = newTeachingClassDto.Level,
            Language = newTeachingClassDto.Language,
            SchoolId = school.Id,
            School = school,
            MainTeacherId = mainTeacherId,
            SubTeachersIds = subTeacherIds,
            StudentsIds = studentIds
        };
    
        await repo.AddTeachingClassAsync(teachingClass, cancellationToken);
        return teachingClass.Id;
    }
}