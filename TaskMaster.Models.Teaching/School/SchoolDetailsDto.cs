namespace TaskMaster.Models.Teaching.School;

public class SchoolDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<SchoolMember> SchoolTeachers { get; set; }
    public IEnumerable<SchoolMember> SchoolAdmins { get; set; }
    public IEnumerable<TeachingClassDto> TeachingClasses { get; set; }
}