using DDDStateMachineExample.Domain.Common.Abstractions;
using DDDStateMachineExample.Domain.Common.Models;
using DDDStateMachineExample.Domain.OrderAggregate.Errors;
using DDDStateMachineExample.Domain.OrderAggregate.ValueObjects;
using DDDStateMachineExample.Domain.OrderAggregate.Workflows;
using JustPlatform.Domain;

namespace DDDStateMachineExample.Domain.OrderAggregate;

public sealed class Order : Entity<long>, IAggregateRoot
{
    public override long Id { get; protected set; }

    public State State { get; protected set; }

    public WorkflowId WorkflowId { get; private set; }

    private Order(long id, State state, WorkflowId workflowId)
    {
        Id = id;
        State = state;
        WorkflowId = workflowId;
    }

    #region FactoryMethods

    public static Order CreateNewInstance(WorkflowId workflowId)
    {
        return new Order(
            id: 0,
            state: new State(State.StateType.New, State.StatusType.New),
            workflowId: workflowId);
    }

    public static Order CreateNewInstance(long id, State state, WorkflowId workflowId)
    {
        return new Order(
            id: id,
            state: state,
            workflowId: workflowId);
    }

    #endregion

    #region public methods

    public async Task<MoveToStateResult<OrderValidationResultType>> ToNextState(
        OrderWorkflowFactory workflowFactory,
        CancellationToken token
    )
    {
        var stateMachine = workflowFactory.GetWorkflow(workflowId: WorkflowId);

        return await stateMachine.MoveToNextState(entityToMove: this, token);
    }
    
    public async Task<MoveToStateResult<OrderValidationResultType>> ToPreviousState(
        OrderWorkflowFactory workflowFactory,
        CancellationToken token
    )
    {
        var stateMachine = workflowFactory.GetWorkflow(workflowId: WorkflowId);

        return await stateMachine.MoveToPreviousState(entityToMove: this, token);
    }
    
    public async Task<MoveToStateResult<OrderValidationResultType>> ToState(
        State.StateType newState,
        OrderWorkflowFactory workflowFactory,
        CancellationToken token
    )
    {
        var stateMachine = workflowFactory.GetWorkflow(workflowId: WorkflowId);

        return await stateMachine.MoveToState(entityToMove: this, newState, token);
    }
    
    internal OrderValidationResult ChangeState(State.StateType newState, State.StatusType newStatus)
    {
        State = new State(newState, newStatus);

        return new OrderValidationResult(false, []);
    }

    #endregion
}