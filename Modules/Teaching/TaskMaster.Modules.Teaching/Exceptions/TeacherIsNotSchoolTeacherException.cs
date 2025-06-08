using TaskMaster.Infrastructure.Exceptions;

namespace TaskMaster.Modules.Teaching.Exceptions;

public sealed class TeacherIsNotSchoolTeacherException(Guid id) 
    : CustomException($"Teacher with ID {id} is not a school teacher.");