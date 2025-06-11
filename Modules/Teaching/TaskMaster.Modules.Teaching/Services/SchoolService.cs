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

        await repo.AddSchoolAsync(school, cancellationToken);
        return school.Id;
    }

    public async Task AddSchoolTeacherAsync(Guid schoolId, Guid teacherId, CancellationToken cancellationToken)
    {
        var school = await repo.GetSchoolAsync(schoolId, cancellationToken)
                     ?? throw new SchoolNotFoundException(schoolId);

        school.SchoolTeachers.Add(new SchoolTeacher
        {
            SchoolId = schoolId,
            TeacherId = teacherId
        });
        await repo.UpdateSchoolAsync(school, cancellationToken);
    }

    public async Task RemoveSchoolTeacherAsync(Guid schoolId, Guid teacherId, CancellationToken cancellationToken)
    {
        var school = await repo.GetSchoolAsync(schoolId, cancellationToken)
                     ?? throw new SchoolNotFoundException(schoolId);

        var schoolTeacher = school.SchoolTeachers.FirstOrDefault(st => st.TeacherId == teacherId);

        if (schoolTeacher is null)
            return;

        school.SchoolTeachers.Remove(schoolTeacher);
        await repo.UpdateSchoolAsync(school, cancellationToken);
    }

    public async Task AddSchoolAdminAsync(Guid schoolId, Guid teacherId, CancellationToken cancellationToken)
    {
        var school = await repo.GetSchoolAsync(schoolId, cancellationToken)
                     ?? throw new SchoolNotFoundException(schoolId);

        school.SchoolAdmins.Add(new SchoolAdmin
        {
            SchoolId = schoolId,
            AdminId = teacherId
        });
        await repo.UpdateSchoolAsync(school, cancellationToken);
    }

    public async Task RemoveSchoolAdminAsync(Guid schoolId, Guid teacherId, CancellationToken cancellationToken)
    {
        var school = await repo.GetSchoolAsync(schoolId, cancellationToken)
                     ?? throw new SchoolNotFoundException(schoolId);

        var schoolAdmin = school.SchoolAdmins.FirstOrDefault(sa => sa.AdminId == teacherId);

        if (schoolAdmin is null)
            return;

        school.SchoolAdmins.Remove(schoolAdmin);
        await repo.UpdateSchoolAsync(school, cancellationToken);
    }

    public async Task<SchoolDetailsDto?> GetSchoolAsync(Guid schoolId, CancellationToken cancellationToken)
    {
        var school = await repo.GetSchoolAsync(schoolId, cancellationToken);

        if (school is null)
            return null;

        var schoolDto = new SchoolDetailsDto
        {
            Id = school.Id,
            Name = school.Name,
            TeachingClasses = school.Classes
                .Select(tc => new TeachingClassDto
                {
                    Id = tc.Id,
                    Name = tc.Name,
                    Level = tc.Level,
                    Language = tc.Language,
                })
        };

        var teachersTasks = school.SchoolTeachers
            .Select(st => queryDispatcher.QueryAsync(new UserByIdQuery(st.TeacherId)))
            .ToList();

        var adminsTasks = school.SchoolAdmins
            .Select(st => queryDispatcher.QueryAsync(new UserByIdQuery(st.AdminId)))
            .ToList();

        var teacherMappingTask = Parallel.ForEachAsync(teachersTasks, cancellationToken, async (task, ct) =>
        {
            var user = await task;
            if (user is not null)
            {
                schoolDto.SchoolTeachers
                    .Add(new SchoolMember
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        UniqueNumber = user.UniqueNumber
                    });
            }
        });

        var adminMappingTask = Parallel.ForEachAsync(adminsTasks, cancellationToken, async (task, ct) =>
        {
            var user = await task;
            if (user is not null)
            {
                schoolDto.SchoolAdmins
                    .Add(new SchoolMember
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Email = user.Email,
                        UniqueNumber = user.UniqueNumber
                    });
            }
        });

        await Task.WhenAll(teacherMappingTask, adminMappingTask);

        return schoolDto;
    }

    public async Task<Guid> CreateTeachingClassAsync(NewTeachingClassDto newTeachingClassDto,
        CancellationToken cancellationToken)
    {
        var school = await repo.GetSchoolAsync(newTeachingClassDto.SchoolId, cancellationToken)
                     ?? throw new SchoolNotFoundException(newTeachingClassDto.SchoolId);

        var teachingClass = new TeachingClass()
        {
            Id = Guid.CreateVersion7(),
            Name = newTeachingClassDto.Name,
            Level = newTeachingClassDto.Level,
            Language = newTeachingClassDto.Language,
            SchoolId = school.Id,
            School = school
        };

        if (newTeachingClassDto.MainTeacherId is not null)
        {
            teachingClass.MainTeacherId = newTeachingClassDto.MainTeacherId.Value;
        }

        teachingClass.SubTeachersIds = newTeachingClassDto.SubTeachersIds.ToList();
        teachingClass.StudentsIds = newTeachingClassDto.StudentsIts.ToList();

        await repo.AddTeachingClassAsync(teachingClass, cancellationToken);
        return teachingClass.Id;
    }

    public async Task<TeachingClassDetailsDto?> GetTeachingClassAsync(Guid classId, CancellationToken cancellationToken)
    {
        var teachingClass = await repo.GetTeachingClassAsync(classId, cancellationToken);

        if (teachingClass is null)
                    return null;
        
        return new TeachingClassDetailsDto
        {
            Id = teachingClass.Id,
            Name = teachingClass.Name,
            Level = teachingClass.Level,
            Language = teachingClass.Language,
        };
    }
}