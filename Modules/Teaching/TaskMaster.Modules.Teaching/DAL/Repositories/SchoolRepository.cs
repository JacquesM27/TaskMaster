using Microsoft.EntityFrameworkCore;
using TaskMaster.Modules.Teaching.Entities;
using TaskMaster.Modules.Teaching.Repositories;

namespace TaskMaster.Modules.Teaching.DAL.Repositories;

internal sealed class SchoolRepository(TeachingDbContext context) : ISchoolRepository
{
    public async Task AddSchoolAsync(School school, CancellationToken cancellationToken)
    {
        await context.Schools.AddAsync(school, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<School?> GetSchoolAsync(Guid id, CancellationToken cancellationToken) 
        => context.Schools.SingleOrDefaultAsync(s => s.Id == id, cancellationToken);

    public async Task AddSchoolAdminAsync(SchoolAdmin schoolAdmin, CancellationToken cancellationToken)
    {
        await context.SchoolAdmins.AddAsync(schoolAdmin, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<SchoolAdmin>> GetSchoolAdminsAsync(Guid schoolId, CancellationToken cancellationToken)
        => await context.SchoolAdmins.Where(sa => sa.SchoolId == schoolId).ToListAsync(cancellationToken);

    public async Task AddTeachingClassAsync(TeachingClass teachingClass, CancellationToken cancellationToken)
    {
        await context.TeachingClasses.AddAsync(teachingClass, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task<TeachingClass?> GetTeachingClassAsync(Guid id, CancellationToken cancellationToken)
        => context.TeachingClasses.SingleOrDefaultAsync(tc => tc.Id == id, cancellationToken);

    public async Task AddSubTeacherAsync(Guid teachingClassId, Guid teacherId, CancellationToken cancellationToken)
    {
        var teachingClass = await context.TeachingClasses.SingleAsync(tc => tc.Id == teachingClassId, cancellationToken);
        teachingClass.SubTeachersIds.Add(teacherId);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddSchoolTeacherAsync(SchoolTeacher schoolTeacher, CancellationToken cancellationToken)
    {
        await context.SchoolTeachers.AddAsync(schoolTeacher, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<SchoolTeacher>> GetSchoolTeachersAsync(Guid schoolId, CancellationToken cancellationToken)
        => await context.SchoolTeachers.Where(st => st.SchoolId == schoolId).ToListAsync(cancellationToken);
}