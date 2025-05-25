namespace TaskMaster.Models.Teaching.School;

public sealed record NewTeachingClassDto(Guid SchoolId, string Name, 
    string Level, string Language, string MainTeacherMail, IEnumerable<string> SubTeachersMails, 
    IEnumerable<string> StudentsMails);
