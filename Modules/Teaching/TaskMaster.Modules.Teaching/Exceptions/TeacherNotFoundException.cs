using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Teaching.Exceptions;

public sealed class TeacherNotFoundException(Guid guid) 
    : CustomException($"Teacher with id: '{guid}' was not found");