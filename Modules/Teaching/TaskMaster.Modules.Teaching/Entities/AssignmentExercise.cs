namespace TaskMaster.Modules.Teaching.Entities;

public class AssignmentExercise
{
    public Guid ExerciseId { get; set; }
    
    public Guid AssignmentId { get; set; }
    public Assignment Assignment { get; set; }
}