using Microsoft.AspNetCore.Builder;
using TaskMaster.Models.Exercises.OpenForm;
using TaskMaster.Modules.Exercises.Extension;

namespace TaskMaster.Modules.Exercises.OpenForm.Endpoints;

internal static class OpenFormEndpoints
{
    internal static WebApplication AddOpenFormEndpoints(this WebApplication app)
    {
        const string route = "open-form-ai";
        const string tag = "OpenFormAi";

        app.MapPostEndpoint<MailQuery, MailResponseOpenForm, Mail>(route, "mail", tag);
        app.MapPostEndpoint<EssayQuery, EssayResponseOpenForm, Essay>(route, "essay", tag);
        app.MapPostEndpoint<SummaryOfTextQuery, SummaryOfTextResponseOpenForm, SummaryOfText>(route,
            "summary-of-text", tag);

        return app;
    }
}