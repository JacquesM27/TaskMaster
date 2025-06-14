namespace TaskMaster.Models.Teaching.Assignment;

public class NewAssignmentDto
{
    public string Name { get; set; }
    public string Description { get; set; }
    public IEnumerable<AssignmentExerciseDto> Exercises { get; set; }
}