namespace TaskMaster.Modules.Teaching.Entities;

public class Assignment
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime DueDate { get; set; }
    public string? Password { get; set; }
    public IEnumerable<AssignmentExercise> Exercises { get; set; }
}