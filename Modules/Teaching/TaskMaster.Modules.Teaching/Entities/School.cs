namespace TaskMaster.Modules.Teaching.Entities;

public class School
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IEnumerable<TeachingClass> Classes { get; set; }
    public IEnumerable<SchoolTeacher> SchoolTeachers { get; set; }
}