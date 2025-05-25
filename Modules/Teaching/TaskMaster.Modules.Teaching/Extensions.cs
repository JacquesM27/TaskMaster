using Microsoft.Extensions.DependencyInjection;
using TaskMaster.Infrastructure.DAL;
using TaskMaster.Modules.Teaching.DAL;
using TaskMaster.Modules.Teaching.DAL.Repositories;
using TaskMaster.Modules.Teaching.Repositories;
using TaskMaster.Modules.Teaching.Services;

namespace TaskMaster.Modules.Teaching;

public static class Extensions
{
    public static IServiceCollection AddTeachingModule(this IServiceCollection services)
    {
        services
            .AddPostgres<TeachingDbContext, DatabaseInitializer>()
            .AddScoped<ISchoolRepository, SchoolRepository>()
            .AddScoped<IAssignmentRepository, AssignmentRepository>()
            .AddScoped<IOpenFormAnswerRepository, OpenFormAnswerRepository>()
            .AddScoped<ISchoolService, SchoolService>();

        return services;
    }
}
