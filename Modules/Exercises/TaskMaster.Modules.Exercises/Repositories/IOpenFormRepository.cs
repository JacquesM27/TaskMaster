using TaskMaster.Models.Exercises.OpenForm;

namespace TaskMaster.Modules.Exercises.Repositories;

public interface IOpenFormRepository
{
    Task<MailDto?> GetMailAsync(Guid id);
    Task<EssayDto?> GetEssayAsync(Guid id);
    Task<SummaryOfTextDto?> GetSummaryOfTextAsync(Guid id);
    
    Task AddMailAsync(Mail exercise, Guid id, bool exerciseHeaderInMotherLanguage,
        string motherLanguage, string targetLanguage, string targetLanguageLevel,
        string? topicsOfSentences, string? grammarSection);

    Task AddEssayAsync(Essay exercise, Guid id, bool exerciseHeaderInMotherLanguage,
        string motherLanguage, string targetLanguage, string targetLanguageLevel,
        string? topicsOfSentences, string? grammarSection);

    Task AddSummaryOfTextAsync(SummaryOfText exercise, Guid id, bool exerciseHeaderInMotherLanguage,
        string motherLanguage, string targetLanguage, string targetLanguageLevel,
        string? topicsOfSentences, string? grammarSection);
}