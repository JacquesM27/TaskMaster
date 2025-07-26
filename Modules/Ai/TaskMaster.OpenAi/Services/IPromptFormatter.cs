using TaskMaster.Models.Exercises.Requests;

namespace TaskMaster.OpenAi.Services;

public interface IPromptFormatter
{
    string FormatExerciseBaseData(ExerciseRequestBase baseData);
    string FormatStartingSystemMessage(string motherLanguage, string targetLanguage);

    string FormatValidationSystemMessage();
}