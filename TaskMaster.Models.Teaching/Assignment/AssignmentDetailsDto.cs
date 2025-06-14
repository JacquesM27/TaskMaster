using TaskMaster.Models.Exercises.OpenForm;

namespace TaskMaster.Models.Teaching.Assignment;

public class AssignmentDetailsDto
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    //TODO: this is a bad idea. will fix that later.
    public IEnumerable<MailDto> MailExercises { get; set; }
    public IEnumerable<EssayDto> EssayExercises { get; set; }
    public IEnumerable<SummaryOfTextDto> SummaryOfTextExercises { get; set; }
    
}