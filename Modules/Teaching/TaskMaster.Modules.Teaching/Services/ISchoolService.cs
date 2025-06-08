using TaskMaster.Models.Teaching.School;

namespace TaskMaster.Modules.Teaching.Services;

public interface ISchoolService
{
    Task<Guid> CreateSchoolAsync(NewSchoolDto newSchoolDto, CancellationToken cancellationToken);
    Task AddSchoolTeacherAsync(Guid schoolId, Guid teacherId, CancellationToken cancellationToken);
    Task RemoveSchoolTeacherAsync(Guid schoolId, Guid teacherId, CancellationToken cancellationToken);
    Task AddSchoolAdminAsync(Guid schoolId, Guid teacherId, CancellationToken cancellationToken);
    Task RemoveSchoolAdminAsync(Guid schoolId, Guid teacherId, CancellationToken cancellationToken);
    Task<SchoolDetailsDto?> GetSchoolAsync(Guid schoolId, CancellationToken cancellationToken);
    Task<Guid> CreateTeachingClassAsync(NewTeachingClassDto newTeachingClassDto, CancellationToken cancellationToken);
    Task<TeachingClassDetailsDto?> GetTeachingClassAsync(Guid classId, CancellationToken cancellationToken);
}