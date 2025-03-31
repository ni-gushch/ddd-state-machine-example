using JustPlatform.Domain;

namespace DDDStateMachineExample.Domain.Common.Abstractions;

public record MoveToStateResult(bool IsError, );

public interface IStateMachine<in TEntity, in TState, TId>
    where TEntity: Entity<TId>
    where TId: struct, IEquatable<TId>
{
    Task<>
}