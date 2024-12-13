using TaskMaster.Models.Exercises.Base;

namespace TaskMaster.Models.Exercises.OpenForm;

public abstract class OpenForm : Exercise
{
}

public class Mail : OpenForm
{
}

public class Essay : OpenForm
{
}

public class SummaryOfText : OpenForm
{
    public string TextToSummary { get; set; }
}