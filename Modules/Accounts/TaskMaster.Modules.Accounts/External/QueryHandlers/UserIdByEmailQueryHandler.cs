using TaskMaster.Abstractions.Queries;
using TaskMaster.Modules.Accounts.Services;
using TasMaster.Queries.Accounts;

namespace TaskMaster.Modules.Accounts.External.QueryHandlers;

internal sealed class UserIdByEmailQueryHandler(IIdentityService identityService) : IQueryHandler<UserIdByEmailQuery, Guid?>
{
    public Task<Guid?> HandleAsync(UserIdByEmailQuery query)
    {
        return identityService.GetIdByEmailAsync(query.Email);
    }
}