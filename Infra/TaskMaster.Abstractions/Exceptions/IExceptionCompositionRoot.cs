namespace TaskMaster.Abstractions.Exceptions;

public interface IExceptionCompositionRoot
{
    ExceptionResponse Map(Exception exception);
}