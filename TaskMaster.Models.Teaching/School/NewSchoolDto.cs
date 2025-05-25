namespace TaskMaster.Models.Teaching.School;

public sealed record NewSchoolDto(string Name, IEnumerable<string> TeachersMails, 
    IEnumerable<string> AdminsMails);