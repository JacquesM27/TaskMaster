namespace TaskMaster.Models.Teaching.School;

public sealed record NewTeachingClassDto(Guid SchoolId, string Name, 
    string Level, string Language, IEnumerable<Guid> SubTeachersIds, IEnumerable<Guid> StudentsIts, 
    Guid? MainTeacherId = null);
