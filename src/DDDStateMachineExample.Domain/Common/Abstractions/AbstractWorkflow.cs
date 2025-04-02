using DDDStateMachineExample.Domain.Common.Models;
using JustPlatform.Domain;

namespace DDDStateMachineExample.Domain.Common.Abstractions;

internal abstract class AbstractWorkflow<TEntity, TId, TStateObject, TStateEnum, TEntityErrorType, TWorkflowTransition> 
    : IWorkflow<TEntity, TId, TStateEnum, TEntityErrorType>
    where TEntity : Entity<TId>, IEntityWithState<TStateObject, TStateEnum>
    where TStateObject : IState<TStateEnum>
    where TStateEnum : Enum
    where TId : struct, IEquatable<TId>
    where TEntityErrorType : Enum
    where TWorkflowTransition : WorkflowTransition<TEntity, TId, TStateEnum, TEntityErrorType>
{
    protected readonly Dictionary<WorkflowTransition<TEntity, TId, TStateEnum, TEntityErrorType>.TransitionKey, TWorkflowTransition> Transitions = new();
    private readonly WorkflowTransitionsPipeline _pipeline = new();

    protected void AddTransition(
        TWorkflowTransition transition,
        MoveDirection? moveDirection = null
    )
    {
        if (Transitions.TryGetValue(transition.Key, out _))
        {
            throw new ArgumentException($"Переход из состояния {transition.FromState} уже был зарегистрирован");
        }

        Transitions[transition.Key] = transition;
        
        if (!moveDirection.HasValue)
            return;

        switch (moveDirection)
        {
            case MoveDirection.Next:
            {
                _pipeline.AddNextState(transition.FromState, transition.ToState);
                break;
            }
            case MoveDirection.Previous:
            {
                _pipeline.AddPreviousState(transition.FromState, transition.ToState);
                break;
            }
            default:
            {
                throw new ArgumentOutOfRangeException(nameof(moveDirection), moveDirection, "Direction is not valid");
            }
        }
    }

    public async Task<MoveToStateResult<TEntityErrorType>> MoveToState(
        TEntity entityToMove,
        TStateEnum newState,
        CancellationToken token
    )
    {
        if (Transitions.TryGetValue(
                new WorkflowTransition<TEntity, TId, TStateEnum, TEntityErrorType>.TransitionKey(entityToMove.State.Current, newState),
                out var transition
            ))
        {
            foreach (var middleware in transition.Middlewares)
            {
                var middlewareResult = await middleware.Invoke(
                    entityToMove,
                    token
                );
                if (middlewareResult.IsError)
                    return middlewareResult;
            }

            var transitionResult = await transition.Invoke(
                entity: entityToMove,
                token: token
            );

            return transitionResult.IsError
                ? transitionResult
                : MoveToStateResult<TEntityErrorType>.Ok;
        }

        return new MoveToStateResult<TEntityErrorType>(
            IsError: true,
            WorkflowProcessError<TEntityErrorType>.TransitionNotFoundError(
                entityToMove.State.Current.ToString(),
                newState.ToString(),
                entityToMove.Id.ToString()
            )
        );
    }

    public async Task<MoveToStateResult<TEntityErrorType>> MoveToNextState(
        TEntity entityToMove,
        CancellationToken token
    )
    {
        var result = _pipeline.GetNextState(entityToMove);
        if (result.Item2 is not null)
            return new MoveToStateResult<TEntityErrorType>(IsError: true, result.Item2);
        if (Transitions.TryGetValue(result.Item1!, out var transition))
            return await DoWorkflowTransition(entityToMove, transition, token);

        return new MoveToStateResult<TEntityErrorType>(IsError: true,
            WorkflowProcessError<TEntityErrorType>.TransitionNotFoundError(
                result.Item1!.TransitionFrom.ToString(),
                result.Item1!.TransitionTo.ToString(),
                entityToMove.Id.ToString()
            ));
    }

    public async Task<MoveToStateResult<TEntityErrorType>> MoveToPreviousState(
        TEntity entityToMove,
        CancellationToken token
    )
    {
        var result = _pipeline.GetPreviousState(entityToMove);
        if (result.Item2 is not null)
            return new MoveToStateResult<TEntityErrorType>(IsError: true, result.Item2);
        if (Transitions.TryGetValue(result.Item1!, out var transition))
            return await DoWorkflowTransition(entityToMove, transition, token);

        return new MoveToStateResult<TEntityErrorType>(IsError: true,
            WorkflowProcessError<TEntityErrorType>.TransitionNotFoundError(
                result.Item1!.TransitionFrom.ToString(),
                result.Item1!.TransitionTo.ToString(),
                entityToMove.Id.ToString()
            ));
    }

    private async Task<MoveToStateResult<TEntityErrorType>> DoWorkflowTransition(
        TEntity entityToMove,
        TWorkflowTransition transition,
        CancellationToken token
    )
    {
        var transitionResult = await transition.Invoke(
            entity: entityToMove,
            token: token
        );

        return transitionResult.IsError
            ? transitionResult
            : MoveToStateResult<TEntityErrorType>.Ok;
    }

    private class WorkflowTransitionsPipeline
    {
        private readonly Dictionary<TStateEnum, TStateEnum> _nextStates = new();
        private readonly Dictionary<TStateEnum, TStateEnum> _previousStates = new();

        public (WorkflowTransition<TEntity, TId, TStateEnum, TEntityErrorType>.TransitionKey?, WorkflowProcessError<TEntityErrorType>?) GetNextState(
            TEntity entityToMove
        )
        {
            if (_nextStates.TryGetValue(entityToMove.State.Current, out var nextState))
                return (
                    new WorkflowTransition<TEntity, TId, TStateEnum, TEntityErrorType>.TransitionKey(entityToMove.State.Current, nextState),
                    null
                );
            return (
                null,
                WorkflowProcessError<TEntityErrorType>.TransitionNotFoundError(
                    entityToMove.State.Current.ToString(),
                    "NOT_FOUND",
                    entityToMove.Id.ToString()
                )
            );
        }

        public (WorkflowTransition<TEntity, TId, TStateEnum, TEntityErrorType>.TransitionKey?, WorkflowProcessError<TEntityErrorType>?) GetPreviousState(
            TEntity entityToMove
        )
        {
            if (_previousStates.TryGetValue(entityToMove.State.Current, out var previousState))
                return (
                    new WorkflowTransition<TEntity, TId, TStateEnum, TEntityErrorType>.TransitionKey(entityToMove.State.Current, previousState),
                    null
                );
            return (
                null,
                WorkflowProcessError<TEntityErrorType>.TransitionNotFoundError(
                    entityToMove.State.Current.ToString(),
                    "NOT_FOUND",
                    entityToMove.Id.ToString()
                )
            );
        }

        public void AddNextState(
            TStateEnum currentState,
            TStateEnum nextState
        )
        {
            if (_nextStates.TryGetValue(currentState, out _))
                throw new Exception(
                    $"Для перехода {currentState.ToString()} уже зарегистрировал шаг в пайплайне."
                );
            _nextStates[currentState] = nextState;
        }

        public void AddPreviousState(
            TStateEnum currentState,
            TStateEnum previousState
        )
        {
            if (_previousStates.TryGetValue(currentState, out _))
                throw new Exception(
                    $"Для перехода {currentState.ToString()} уже зарегистрировал шаг в пайплайне."
                );
            _previousStates[currentState] = previousState;
        }
    }
}