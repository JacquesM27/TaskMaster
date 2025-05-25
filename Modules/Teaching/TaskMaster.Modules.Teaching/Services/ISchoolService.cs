using TaskMaster.Models.Teaching.School;

namespace TaskMaster.Modules.Teaching.Services;

public interface ISchoolService
{
    Task<Guid> CreateSchoolAsync(NewSchoolDto newSchoolDto, CancellationToken cancellationToken);
    Task<Guid> CreateTeachingClassAsync(NewTeachingClassDto newTeachingClassDto, CancellationToken cancellationToken);
}