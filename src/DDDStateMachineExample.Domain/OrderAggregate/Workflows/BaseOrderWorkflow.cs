using DDDStateMachineExample.Domain.Common.Abstractions;
using DDDStateMachineExample.Domain.Common.Models;
using DDDStateMachineExample.Domain.OrderAggregate.Errors;
using DDDStateMachineExample.Domain.OrderAggregate.ValueObjects;

namespace DDDStateMachineExample.Domain.OrderAggregate.Workflows;

internal abstract class BaseOrderWorkflow 
    : AbstractWorkflow<Order, long, State, State.StateType, OrderValidationResultType, BaseOrderWorkflow.OrderStateTransition>
{
    internal class OrderStateTransition(
        State.StateType fromState,
        State.StateType toState,
        Func<Order, CancellationToken, Task<MoveToStateResult<OrderValidationResultType>>> toStateFunction,
        Func<Order, CancellationToken, Task<MoveToStateResult<OrderValidationResultType>>>[]? toStateMiddlewares = null
    )
        : WorkflowTransition<Order, long, State.StateType, OrderValidationResultType>(fromState, toState,
            toStateFunction, toStateMiddlewares)
    {
    }
}