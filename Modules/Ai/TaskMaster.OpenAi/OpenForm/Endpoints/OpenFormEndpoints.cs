using Microsoft.AspNetCore.Builder;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.OpenAi.Extensions;
using TaskMaster.OpenAi.OpenForm.Queries;

namespace TaskMaster.OpenAi.OpenForm.Endpoints;

internal static class OpenFormEndpoints
{
    internal static WebApplication AddOpenFormEndpoints(this WebApplication app)
    {
        const string route = "open-form";
        const string tag = "OpenForm";

        app.MapPostEndpoint<MailQuery, MailResponseOpenForm, Mail>(route, "mail", tag);
        app.MapPostEndpoint<EssayQuery, EssayResponseOpenForm, Essay>(route, "essay", tag);
        app.MapPostEndpoint<SummaryOfTextQuery, SummaryOfTextResponseOpenForm, SummaryOfText>(route,
            "summary-of-text", tag);

        return app;
    }
}