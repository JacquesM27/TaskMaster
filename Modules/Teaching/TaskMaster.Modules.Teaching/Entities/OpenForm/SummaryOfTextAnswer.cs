namespace TaskMaster.Modules.Teaching.Entities.OpenForm;

public class SummaryOfTextAnswer
{
    public Guid Id { get; set; }
    public Guid StudentId { get; set; }
    public string WrittenSummary { get; set; }
    public Guid AssignmentExerciseId { get; set; }

    public bool Verified { get; set; }
    public int Points { get; set; }
    public IEnumerable<Mistake> Mistakes { get; set; }
}