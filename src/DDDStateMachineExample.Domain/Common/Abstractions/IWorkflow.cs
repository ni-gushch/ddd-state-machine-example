using DDDStateMachineExample.Domain.Common.Models;
using JustPlatform.Domain;

namespace DDDStateMachineExample.Domain.Common.Abstractions;

public record MoveToStateResult<TEntityErrorType>(
    bool IsError,
    WorkflowProcessError<TEntityErrorType>? WorkflowProcessErrors
)
    where TEntityErrorType : Enum
{
    public static MoveToStateResult<TEntityErrorType> Ok => new(false, null);
};

public interface IWorkflow<in TEntity, TId, in TState, TEntityErrorType>
    where TEntity : Entity<TId>
    where TState : Enum
    where TId : struct, IEquatable<TId>
    where TEntityErrorType : Enum
{
    Task<MoveToStateResult<TEntityErrorType>> MoveToState(
        TEntity entityToMove,
        TState newState,
        CancellationToken token
    );
    
    Task<MoveToStateResult<TEntityErrorType>> MoveToNextState(
        TEntity entityToMove,
        CancellationToken token
    );

    Task<MoveToStateResult<TEntityErrorType>> MoveToPreviousState(
        TEntity entityToMove,
        CancellationToken token
    );
}