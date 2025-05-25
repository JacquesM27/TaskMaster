namespace TaskMaster.Modules.Teaching.Entities;

public class School
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public IList<TeachingClass> Classes { get; set; }
    public IList<SchoolTeacher> SchoolTeachers { get; set; }
    public IList<SchoolAdmin> SchoolAdmins { get; set; }
}