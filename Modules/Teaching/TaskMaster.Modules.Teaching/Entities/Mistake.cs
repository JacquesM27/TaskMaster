namespace TaskMaster.Modules.Teaching.Entities;

public class Mistake
{
    public Guid Id { get; set; }
    public string StudentsAnswer { get; set; }
    public string CorrectAnswer { get; set; }
    public string? Explanation { get; set; }
}