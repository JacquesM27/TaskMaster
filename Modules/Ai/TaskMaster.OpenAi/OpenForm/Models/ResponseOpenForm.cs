using TaskMaster.OpenAi.Models;

namespace TaskMaster.OpenAi.OpenForm.Models;

internal abstract class ResponseOpenForm<TExercise> : ExerciseResponse<TExercise>
    where TExercise : OpenForm
{
}

internal class MailResponseOpenForm : ResponseOpenForm<Mail>;

internal class EssayResponseOpenForm : ResponseOpenForm<Essay>;

internal class SummaryOfTextResponseOpenForm : ResponseOpenForm<SummaryOfText>;