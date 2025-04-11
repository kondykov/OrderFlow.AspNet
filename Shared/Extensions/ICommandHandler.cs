namespace OrderFlow.Shared.Extensions;

public interface ICommandHandler<in TIn, TOut>
{
    Task<TOut> Handle(TIn command);
}