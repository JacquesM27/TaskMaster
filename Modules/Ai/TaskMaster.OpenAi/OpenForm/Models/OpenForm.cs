using TaskMaster.OpenAi.Models;

namespace TaskMaster.OpenAi.OpenForm.Models;

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