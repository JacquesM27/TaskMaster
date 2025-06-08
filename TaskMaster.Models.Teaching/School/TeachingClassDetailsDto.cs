namespace TaskMaster.Models.Teaching.School;

public class TeachingClassDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Level { get; set; }
    public string Language { get; set; }
    public SchoolMember MainTeacher { get; set; }
    public IEnumerable<SchoolMember> SubTeachers { get; set; }
    public IEnumerable<SchoolMember> Students { get; set; }
}