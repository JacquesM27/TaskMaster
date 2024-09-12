﻿using TaskMaster.OpenAi.Models;

namespace TaskMaster.OpenAi.Services;

internal interface IPromptFormatter
{
    string FormatExerciseBaseData(ExerciseQueryBase baseData);
    string FormatStartingSystemMessage(string motherLanguage, string targetLanguage);

    string FormatValidationSystemMessage();
}