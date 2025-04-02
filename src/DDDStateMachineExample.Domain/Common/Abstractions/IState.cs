namespace DDDStateMachineExample.Domain.Common.Abstractions;

public interface IState<out TState>
    where TState: Enum
{
    public TState Current { get; }
}