using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Modules.Exercises.Repositories;

namespace TaskMaster.Modules.Exercises.Services;

internal sealed class OpenFormService(IOpenFormRepository openFormRepository) : IOpenFormService
{
    public async Task<MailDto?> GetMailAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await openFormRepository.GetMailAsync(id, cancellationToken);
        
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

    public async Task<EssayDto?> GetEssayAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await openFormRepository.GetEssayAsync(id, cancellationToken);
        
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

    public async Task<SummaryOfTextDto?> GetSummaryOfTextAsync(Guid id, CancellationToken cancellationToken)
    {
        var entity = await openFormRepository.GetSummaryOfTextAsync(id, cancellationToken);
        
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

    public async Task<Guid> AddMailAsync(MailDto dto, CancellationToken cancellationToken)
    {
        var id = Guid.CreateVersion7();
        var mailEntity = new Entities.Mail()
        {
            Exercise = dto.Exercise,
            Id = id,
            ExerciseHeaderInMotherLanguage = dto.ExerciseHeaderInMotherLanguage,
            MotherLanguage = dto.MotherLanguage,
            TargetLanguage = dto.TargetLanguage,
            TargetLanguageLevel = dto.TargetLanguageLevel,
            TopicsOfSentences = dto.TopicsOfSentences,
            GrammarSection = dto.GrammarSection,
            VerifiedByTeacher = false
        };

        await openFormRepository.AddMailAsync(mailEntity, cancellationToken);
        return id;
    }

    public async Task<Guid> AddEssayAsync(EssayDto dto, CancellationToken cancellationToken)
    {
        var id = Guid.CreateVersion7();
        var essayEntity = new Entities.Essay()
        {
            Exercise = dto.Exercise,
            Id = id,
            ExerciseHeaderInMotherLanguage = dto.ExerciseHeaderInMotherLanguage,
            MotherLanguage = dto.MotherLanguage,
            TargetLanguage = dto.TargetLanguage,
            TargetLanguageLevel = dto.TargetLanguageLevel,
            TopicsOfSentences = dto.TopicsOfSentences,
            GrammarSection = dto.GrammarSection,
            VerifiedByTeacher = false
        };

        await openFormRepository.AddEssayAsync(essayEntity, cancellationToken);
        return id;
    }

    public async Task<Guid> AddSummaryOfTextAsync(SummaryOfTextDto dto, CancellationToken cancellationToken)
    {
        var id = Guid.CreateVersion7();
        var summaryOfTextEntity = new Entities.SummaryOfText()
        {
            Exercise = dto.Exercise,
            Id = id,
            ExerciseHeaderInMotherLanguage = dto.ExerciseHeaderInMotherLanguage,
            MotherLanguage = dto.MotherLanguage,
            TargetLanguage = dto.TargetLanguage,
            TargetLanguageLevel = dto.TargetLanguageLevel,
            TopicsOfSentences = dto.TopicsOfSentences,
            GrammarSection = dto.GrammarSection,
            VerifiedByTeacher = false
        };

        await openFormRepository.AddSummaryOfTextAsync(summaryOfTextEntity, cancellationToken);
        return id;
    }
}