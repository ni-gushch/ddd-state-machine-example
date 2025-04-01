using DDDStateMachineExample.Domain.Common.Abstractions;
using DDDStateMachineExample.Domain.Common.Models;
using DDDStateMachineExample.Domain.OrderAggregate.Errors;
using DDDStateMachineExample.Domain.OrderAggregate.ValueObjects;

namespace DDDStateMachineExample.Domain.OrderAggregate.Workflows;

internal abstract class OrderWorkflow : IWorkflow<Order, State.StateType, long, OrderValidationResultType>
{
    protected readonly Dictionary<OrderStateTransition.TransitionKey, OrderStateTransition> Transitions = new();
    private readonly WorkflowTransitionsPipeline _pipeline = new();

    protected void AddTransition(
        OrderStateTransition transition,
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

    public async Task<MoveToStateResult<OrderValidationResultType>> MoveToState(
        Order entityToMove,
        State.StateType newState,
        CancellationToken token
    )
    {
        if (Transitions.TryGetValue(
                new OrderStateTransition.TransitionKey(entityToMove.State.Current, newState),
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
                : MoveToStateResult<OrderValidationResultType>.Ok;
        }

        return new MoveToStateResult<OrderValidationResultType>(
            IsError: true,
            WorkflowProcessError<OrderValidationResultType>.TransitionNotFoundError(
                entityToMove.State.Current.ToString(),
                newState.ToString(),
                entityToMove.Id.ToString()
            )
        );
    }

    public async Task<MoveToStateResult<OrderValidationResultType>> MoveToNextState(
        Order entityToMove,
        CancellationToken token
    )
    {
        var result = _pipeline.GetNextState(entityToMove);
        if (result.Item2 is not null)
            return new MoveToStateResult<OrderValidationResultType>(IsError: true, result.Item2);
        if (Transitions.TryGetValue(result.Item1!, out var transition))
            return await DoWorkflowTransition(entityToMove, transition, token);

        return new MoveToStateResult<OrderValidationResultType>(IsError: true,
            WorkflowProcessError<OrderValidationResultType>.TransitionNotFoundError(
                result.Item1!.TransitionFrom.ToString(),
                result.Item1!.TransitionTo.ToString(),
                entityToMove.Id.ToString()
            ));
    }

    public async Task<MoveToStateResult<OrderValidationResultType>> MoveToPreviousState(
        Order entityToMove,
        CancellationToken token
    )
    {
        var result = _pipeline.GetPreviousState(entityToMove);
        if (result.Item2 is not null)
            return new MoveToStateResult<OrderValidationResultType>(IsError: true, result.Item2);
        if (Transitions.TryGetValue(result.Item1!, out var transition))
            return await DoWorkflowTransition(entityToMove, transition, token);

        return new MoveToStateResult<OrderValidationResultType>(IsError: true,
            WorkflowProcessError<OrderValidationResultType>.TransitionNotFoundError(
                result.Item1!.TransitionFrom.ToString(),
                result.Item1!.TransitionTo.ToString(),
                entityToMove.Id.ToString()
            ));
    }

    private async Task<MoveToStateResult<OrderValidationResultType>> DoWorkflowTransition(
        Order entityToMove,
        OrderStateTransition transition,
        CancellationToken token
    )
    {
        var transitionResult = await transition.Invoke(
            entity: entityToMove,
            token: token
        );

        return transitionResult.IsError
            ? transitionResult
            : MoveToStateResult<OrderValidationResultType>.Ok;
    }

    protected class OrderStateTransition(
        State.StateType fromState,
        State.StateType toState,
        Func<Order, CancellationToken, Task<MoveToStateResult<OrderValidationResultType>>> toStateFunction,
        Func<Order, CancellationToken, Task<MoveToStateResult<OrderValidationResultType>>>[]? toStateMiddlewares = null
    )
        : WorkflowTransition<Order, long, State.StateType, OrderValidationResultType>(fromState, toState,
            toStateFunction)
    {
        public readonly Func<Order, CancellationToken, Task<MoveToStateResult<OrderValidationResultType>>>[]
            Middlewares =
                toStateMiddlewares ?? [];
    }

    private class WorkflowTransitionsPipeline
    {
        private readonly Dictionary<State.StateType, State.StateType> _nextStates = new();
        private readonly Dictionary<State.StateType, State.StateType> _previousStates = new();

        public (OrderStateTransition.TransitionKey?, WorkflowProcessError<OrderValidationResultType>?) GetNextState(
            Order entityToMove
        )
        {
            if (_nextStates.TryGetValue(entityToMove.State.Current, out var nextState))
                return (
                    new OrderStateTransition.TransitionKey(entityToMove.State.Current, nextState),
                    null
                );
            return (
                null,
                WorkflowProcessError<OrderValidationResultType>.TransitionNotFoundError(
                    entityToMove.State.Current.ToString(),
                    "NOT_FOUND",
                    entityToMove.Id.ToString()
                )
            );
        }

        public (OrderStateTransition.TransitionKey?, WorkflowProcessError<OrderValidationResultType>?) GetPreviousState(
            Order entityToMove
        )
        {
            if (_previousStates.TryGetValue(entityToMove.State.Current, out var previousState))
                return (
                    new OrderStateTransition.TransitionKey(entityToMove.State.Current, previousState),
                    null
                );
            return (
                null,
                WorkflowProcessError<OrderValidationResultType>.TransitionNotFoundError(
                    entityToMove.State.Current.ToString(),
                    "NOT_FOUND",
                    entityToMove.Id.ToString()
                )
            );
        }

        public void AddNextState(
            State.StateType currentState,
            State.StateType nextState
        )
        {
            if (_nextStates.TryGetValue(currentState, out _))
                throw new Exception(
                    $"Для перехода {currentState.ToString()} уже зарегистрировал шаг в пайплайне."
                );
            _nextStates[currentState] = nextState;
        }

        public void AddPreviousState(
            State.StateType currentState,
            State.StateType previousState
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