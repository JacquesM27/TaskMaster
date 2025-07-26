namespace TaskMaster.Models.Teaching.Assignment;

public class NewClassAssignmentWithoutAssignmentDto
{
    // class assignment part
    public DateTime DueDate { get; set; }
    public string? Password { get; set; }
    public Guid TeachingClassId { get; set; }
    
    // assignment part
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<AssignmentExerciseDto> Exercises { get; set; }
}