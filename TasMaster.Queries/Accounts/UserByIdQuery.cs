using TaskMaster.Abstractions.Queries;

namespace TasMaster.Queries.Accounts;

//TODO: Consider another way to transfer data between modules
public sealed record UserByIdQuery(Guid UserId) : IQuery<User?>;

public sealed record User(Guid Id, string FirstName, string LastName, string Email, string UniqueNumber);