namespace TaskMaster.Modules.Teaching.Entities;

public class AssignmentExercise
{
    public Guid Id { get; set; }
    public Guid ExerciseId { get; set; }
    public string ExerciseType { get; set; }
    public Guid AssignmentId { get; set; }
    
    public Assignment Assignment { get; set; }
}