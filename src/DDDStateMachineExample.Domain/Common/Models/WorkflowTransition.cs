using DDDStateMachineExample.Domain.Common.Abstractions;
using JustPlatform.Domain;

namespace DDDStateMachineExample.Domain.Common.Models;

internal abstract class WorkflowTransition<TEntity, TId, TState, TWorkflowProcessErrorType>
    where TEntity : Entity<TId>
    where TId : struct, IEquatable<TId>
    where TState : Enum
    where TWorkflowProcessErrorType : Enum
{
    public TState FromState { get; }
    public TState ToState { get; }

    public TransitionKey Key { get; }

    private Func<TEntity, CancellationToken, Task<MoveToStateResult<TWorkflowProcessErrorType>>> ToStateFunction { get; }

    public Func<TEntity, CancellationToken, Task<MoveToStateResult<TWorkflowProcessErrorType>>>[] Middlewares { get; }

    public async Task<MoveToStateResult<TWorkflowProcessErrorType>> Invoke(
        TEntity entity,
        CancellationToken token
    ) => await ToStateFunction.Invoke(entity, token);

    protected WorkflowTransition(
        TState fromState,
        TState toState,
        Func<TEntity, CancellationToken, Task<MoveToStateResult<TWorkflowProcessErrorType>>> toStateFunction,
        Func<TEntity, CancellationToken, Task<MoveToStateResult<TWorkflowProcessErrorType>>>[]? toStateMiddlewares = null
    )
    {
        FromState = fromState;
        ToState = toState;
        Key = new TransitionKey(fromState, toState);
        this.ToStateFunction = toStateFunction;
        Middlewares = toStateMiddlewares ?? [];
    }

    public record TransitionKey(TState TransitionFrom, TState TransitionTo);
}
