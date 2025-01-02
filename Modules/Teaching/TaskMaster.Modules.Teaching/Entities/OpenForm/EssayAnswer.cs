namespace TaskMaster.Modules.Teaching.Entities.OpenForm;

public class EssayAnswer
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string WrittenEssay { get; set; }
    public Guid AssignmentExerciseId { get; set; }

    public bool Verified { get; set; }
    public int Points { get; set; }
    public IList<Mistake> Mistakes { get; set; }
}