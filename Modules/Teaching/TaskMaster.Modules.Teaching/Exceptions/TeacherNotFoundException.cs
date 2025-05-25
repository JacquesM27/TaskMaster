using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Teaching.Exceptions;

public sealed class TeacherNotFoundException(string email) 
    : CustomException($"Teacher with email: '{email}' was not found");