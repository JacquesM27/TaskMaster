namespace TaskMaster.Modules.Teaching.Entities;

public class SchoolTeacher
{
    public Guid SchoolId { get; set; }
    public School School { get; set; }

    public Guid TeacherId { get; set; }
}