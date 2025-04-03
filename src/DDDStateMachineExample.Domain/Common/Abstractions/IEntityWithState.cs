namespace DDDStateMachineExample.Domain.Common.Abstractions;

public interface IEntityWithState<out TStateObject, TStateEnum>
    where TStateObject : IState<TStateEnum>
    where TStateEnum : Enum
{
     TStateObject State { get; }
}