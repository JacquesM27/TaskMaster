using Microsoft.EntityFrameworkCore;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Modules.Exercises.Repositories;

namespace TaskMaster.Modules.Exercises.DAL.Repositories;

internal sealed class OpenFormRepository(ExercisesDbContext context) : IOpenFormRepository
{
    public async Task<MailDto?> GetMailAsync(Guid id)
    {
        var entity = await context.Mails
            .SingleOrDefaultAsync(m => m.Id == id)
            .ConfigureAwait(false);
        
        if (entity is null)
            return null;

        var result = new MailDto
        {
            Id = entity.Id,
            Exercise = entity.Exercise,
            GrammarSection = entity.GrammarSection,
            MotherLanguage = entity.MotherLanguage,
            TargetLanguage = entity.TargetLanguage,
            TargetLanguageLevel = entity.TargetLanguageLevel,
            TopicsOfSentences = entity.TopicsOfSentences,
            ExerciseHeaderInMotherLanguage = entity.ExerciseHeaderInMotherLanguage,
            VerifiedByTeacher = entity.VerifiedByTeacher
        };
        return result;
    }

    public async Task<EssayDto?> GetEssayAsync(Guid id)
    {
        var entity = await context.Essays
            .SingleOrDefaultAsync(m => m.Id == id)
            .ConfigureAwait(false);
        
        if (entity is null)
            return null;

        var result = new EssayDto
        {
            Id = entity.Id,
            Exercise = entity.Exercise,
            GrammarSection = entity.GrammarSection,
            MotherLanguage = entity.MotherLanguage,
            TargetLanguage = entity.TargetLanguage,
            TargetLanguageLevel = entity.TargetLanguageLevel,
            TopicsOfSentences = entity.TopicsOfSentences,
            ExerciseHeaderInMotherLanguage = entity.ExerciseHeaderInMotherLanguage,
            VerifiedByTeacher = entity.VerifiedByTeacher
        };
        return result;
    }

    public async Task<SummaryOfTextDto?> GetSummaryOfTextAsync(Guid id)
    {
        var entity = await context.SummariesOfText
            .SingleOrDefaultAsync(m => m.Id == id)
            .ConfigureAwait(false);
        
        if (entity is null)
            return null;

        var result = new SummaryOfTextDto
        {
            Id = entity.Id,
            Exercise = entity.Exercise,
            GrammarSection = entity.GrammarSection,
            MotherLanguage = entity.MotherLanguage,
            TargetLanguage = entity.TargetLanguage,
            TargetLanguageLevel = entity.TargetLanguageLevel,
            TopicsOfSentences = entity.TopicsOfSentences,
            ExerciseHeaderInMotherLanguage = entity.ExerciseHeaderInMotherLanguage,
            VerifiedByTeacher = entity.VerifiedByTeacher
        };
        return result;
    }

    public async Task AddMailAsync(Mail exercise, Guid id, bool exerciseHeaderInMotherLanguage, string motherLanguage,
        string targetLanguage, string targetLanguageLevel, string? topicsOfSentences, string? grammarSection)
    {
        var mailEntity = new Entities.Mail()
        {
            Exercise = exercise,
            Id = id,
            ExerciseHeaderInMotherLanguage = exerciseHeaderInMotherLanguage,
            MotherLanguage = motherLanguage,
            TargetLanguage = targetLanguage,
            TargetLanguageLevel = targetLanguageLevel,
            TopicsOfSentences = topicsOfSentences,
            GrammarSection = grammarSection,
            VerifiedByTeacher = false
        };
        await context.Mails.AddAsync(mailEntity);
        await context.SaveChangesAsync();
    }

    public async Task AddEssayAsync(Essay exercise, Guid id, bool exerciseHeaderInMotherLanguage, string motherLanguage,
        string targetLanguage, string targetLanguageLevel, string? topicsOfSentences, string? grammarSection)
    {
        var essayEntity = new Entities.Essay()
        {
            Exercise = exercise,
            Id = id,
            ExerciseHeaderInMotherLanguage = exerciseHeaderInMotherLanguage,
            MotherLanguage = motherLanguage,
            TargetLanguage = targetLanguage,
            TargetLanguageLevel = targetLanguageLevel,
            TopicsOfSentences = topicsOfSentences,
            GrammarSection = grammarSection,
            VerifiedByTeacher = false
        };
        await context.Essays.AddAsync(essayEntity);
        await context.SaveChangesAsync();
    }

    public async Task AddSummaryOfTextAsync(SummaryOfText exercise, Guid id, bool exerciseHeaderInMotherLanguage, string motherLanguage,
        string targetLanguage, string targetLanguageLevel, string? topicsOfSentences, string? grammarSection)
    {
        var summaryOfTextEntity = new Entities.SummaryOfText()
        {
            Exercise = exercise,
            Id = id,
            ExerciseHeaderInMotherLanguage = exerciseHeaderInMotherLanguage,
            MotherLanguage = motherLanguage,
            TargetLanguage = targetLanguage,
            TargetLanguageLevel = targetLanguageLevel,
            TopicsOfSentences = topicsOfSentences,
            GrammarSection = grammarSection,
            VerifiedByTeacher = false
        };
        await context.SummariesOfText.AddAsync(summaryOfTextEntity);
        await context.SaveChangesAsync();
    }
}