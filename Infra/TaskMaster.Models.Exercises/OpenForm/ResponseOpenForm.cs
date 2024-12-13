using TaskMaster.Models.Exercises.Base;

namespace TaskMaster.Models.Exercises.OpenForm;

public abstract class ResponseOpenForm<TExercise> : ExerciseResponse<TExercise>
    where TExercise : OpenForm
{
}

public class MailResponseOpenForm : ResponseOpenForm<Mail>;

public class EssayResponseOpenForm : ResponseOpenForm<Essay>;

public class SummaryOfTextResponseOpenForm : ResponseOpenForm<SummaryOfText>;