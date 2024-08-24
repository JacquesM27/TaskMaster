namespace TaskMaster.Abstractions.Exceptions;

public interface IExceptionToResponseMapper
{
    ExceptionResponse? Map(Exception exception);
}