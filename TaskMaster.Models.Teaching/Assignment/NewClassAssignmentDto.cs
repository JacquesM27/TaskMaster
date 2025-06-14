namespace TaskMaster.Models.Teaching.Assignment;

public class NewClassAssignmentDto
{
    public DateTime DueDate { get; set; }
    public string? Password { get; set; }
    public Guid TeachingClassId { get; set; }
    public Guid AssignmentId { get; set; }
}