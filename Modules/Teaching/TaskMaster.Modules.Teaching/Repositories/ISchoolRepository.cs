using TaskMaster.Modules.Teaching.Entities;

namespace TaskMaster.Modules.Teaching.Repositories;

public interface ISchoolRepository
{
    Task AddSchoolAsync(School school, CancellationToken cancellationToken);
    Task<School?> GetSchoolAsync(Guid id, CancellationToken cancellationToken);
    Task UpdateSchoolAsync(School school, CancellationToken cancellationToken);
    
    Task AddSchoolAdminAsync(SchoolAdmin schoolAdmin, CancellationToken cancellationToken);
    Task<IEnumerable<SchoolAdmin>> GetSchoolAdminsAsync(Guid schoolId, CancellationToken cancellationToken);
    
    Task AddTeachingClassAsync(TeachingClass teachingClass, CancellationToken cancellationToken);
    Task<TeachingClass?> GetTeachingClassAsync(Guid id, CancellationToken cancellationToken);
    Task AddSubTeacherAsync(Guid teachingClassId, Guid teacherId, CancellationToken cancellationToken);
    
    Task AddSchoolTeacherAsync(SchoolTeacher schoolTeacher, CancellationToken cancellationToken);
    Task<IEnumerable<SchoolTeacher>> GetSchoolTeachersAsync(Guid schoolId, CancellationToken cancellationToken);
    
}