namespace TaskMaster.Models.Teaching.Assignment;

public class ClassAssignmentDetailsDto
{
    public Guid Id { get; set; }
    public DateTime DueDate { get; set; }
    public AssignmentDetailsDto Assignment { get; set; }
}