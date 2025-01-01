namespace TaskMaster.Modules.Teaching.Entities;

public class SchoolAdmin
{
    public Guid SchoolId { get; set; }
    public School School { get; set; }

    public Guid AdminId { get; set; }
}