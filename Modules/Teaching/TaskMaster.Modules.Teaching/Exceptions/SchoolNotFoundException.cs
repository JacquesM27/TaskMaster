using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Teaching.Exceptions;

public sealed class SchoolNotFoundException(Guid id) 
    : CustomException($"School with id: '{id}' was not found");