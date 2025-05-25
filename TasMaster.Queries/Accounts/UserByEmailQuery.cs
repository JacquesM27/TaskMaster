using TaskMaster.Abstractions.Queries;

namespace TasMaster.Queries.Accounts;

public sealed record UserIdByEmailQuery(string Email) : IQuery<Guid?>;