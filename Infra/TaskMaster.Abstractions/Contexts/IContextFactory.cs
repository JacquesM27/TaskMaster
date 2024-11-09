namespace TaskMaster.Abstractions.Contexts;

public interface IContextFactory
{
    IContext Create();
}