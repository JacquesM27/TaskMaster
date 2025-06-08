using TaskMaster.Abstractions.Queries;
using TaskMaster.Modules.Accounts.Services;
using TasMaster.Queries.Accounts;

namespace TaskMaster.Modules.Accounts.External.QueryHandlers;

internal sealed class UserByIdQueryHandler(IIdentityService identityService) : IQueryHandler<UserByIdQuery, User?>
{
    public async Task<User?> HandleAsync(UserByIdQuery query)
    {
        var user = await identityService.GetAsync(query.UserId);
        
        if (user is null)
            return null;
        
        return new User(user.Id, user.FirstName, user.LastName, user.Email, user.UniqueNumber);
    }
}