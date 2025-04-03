using DDDStateMachineExample.Domain.Common.Abstractions;
using DDDStateMachineExample.Domain.Common.Models;
using DDDStateMachineExample.Domain.OrderAggregate.Errors;
using DDDStateMachineExample.Domain.OrderAggregate.ValueObjects;

namespace DDDStateMachineExample.Domain.OrderAggregate.Workflows;

internal class FirstOrderWorkflowProcess : BaseOrderWorkflow
{
    public FirstOrderWorkflowProcess()
    {
        #region New

        AddTransition(
            new OrderStateTransition(
                fromState: State.StateType.New,
                toState: State.StateType.EditPropertiesStepOne,
                toStateFunction: FromNew_ToEditPropertiesStepOne
            ),
            MoveDirection.Next
        );
        AddTransition(
            new OrderStateTransition(
                fromState: State.StateType.EditPropertiesStepOne,
                toState: State.StateType.New,
                toStateFunction: FromEditPropertiesStepOne_ToNew
            ),
            MoveDirection.Previous
        );
        AddTransition(
            new OrderStateTransition(
                fromState: State.StateType.New,
                toState: State.StateType.Cancellation,
                toStateFunction: FromNew_ToCancellation
            )
        );

        #endregion

        #region EditPropertiesStepOne

        AddTransition(
            new OrderStateTransition(
                fromState: State.StateType.EditPropertiesStepOne,
                toState: State.StateType.WaitingForRegistration,
                toStateFunction: FromEditPropertiesStepOne_ToWaitingForRegistration
            ),
            MoveDirection.Next
        );
        AddTransition(
            new OrderStateTransition(
                fromState: State.StateType.EditPropertiesStepOne,
                toState: State.StateType.Cancellation,
                toStateFunction: FromEditPropertiesStepOne_ToCancellation
            )
        );

        #endregion

        #region WaitingForRegistration

        AddTransition(
            new OrderStateTransition(
                fromState: State.StateType.WaitingForRegistration,
                toState: State.StateType.RegistrationSuccess,
                toStateFunction: FromWaitingForRegistration_ToRegistrationSuccess
            )
        );
        AddTransition(
            new OrderStateTransition(
                fromState: State.StateType.WaitingForRegistration,
                toState: State.StateType.RegistrationRejected,
                toStateFunction: FromWaitingForRegistration_ToRegistrationRejected
            )
        );

        #endregion
    }

    private async Task<MoveToStateResult<OrderValidationResultType>> FromNew_ToEditPropertiesStepOne(
        Order entity,
        CancellationToken token
    )
    {
        var result = entity.ChangeState(State.StateType.EditPropertiesStepOne, State.StatusType.New);
        if (result.IsError)
            return await Task.FromResult(
                new MoveToStateResult<OrderValidationResultType>(IsError: true,
                    new WorkflowProcessError<OrderValidationResultType>(result.Errors)));

        return MoveToStateResult<OrderValidationResultType>.Ok;
    }
    
    private async Task<MoveToStateResult<OrderValidationResultType>> FromEditPropertiesStepOne_ToNew(
        Order entity,
        CancellationToken token
    )
    {
        var result = entity.ChangeState(State.StateType.New, State.StatusType.New);
        if (result.IsError)
            return await Task.FromResult(
                new MoveToStateResult<OrderValidationResultType>(IsError: true,
                    new WorkflowProcessError<OrderValidationResultType>(result.Errors)));

        return MoveToStateResult<OrderValidationResultType>.Ok;
    }
    
    private async Task<MoveToStateResult<OrderValidationResultType>> FromNew_ToCancellation(
        Order entity,
        CancellationToken token
    )
    {
        var result = entity.ChangeState(State.StateType.Cancellation, State.StatusType.Cancellation);
        if (result.IsError)
            return await Task.FromResult(
                new MoveToStateResult<OrderValidationResultType>(IsError: true,
                    new WorkflowProcessError<OrderValidationResultType>(result.Errors)));

        return MoveToStateResult<OrderValidationResultType>.Ok;
    }
    
    private async Task<MoveToStateResult<OrderValidationResultType>> FromEditPropertiesStepOne_ToWaitingForRegistration(
        Order entity,
        CancellationToken token
    )
    {
        var result = entity.ChangeState(State.StateType.WaitingForRegistration, State.StatusType.Registration);
        if (result.IsError)
            return await Task.FromResult(
                new MoveToStateResult<OrderValidationResultType>(IsError: true,
                    new WorkflowProcessError<OrderValidationResultType>(result.Errors)));

        return MoveToStateResult<OrderValidationResultType>.Ok;
    }
    
    private async Task<MoveToStateResult<OrderValidationResultType>> FromEditPropertiesStepOne_ToCancellation(
        Order entity,
        CancellationToken token
    )
    {
        var result = entity.ChangeState(State.StateType.Cancellation, State.StatusType.Cancellation);
        if (result.IsError)
            return await Task.FromResult(
                new MoveToStateResult<OrderValidationResultType>(IsError: true,
                    new WorkflowProcessError<OrderValidationResultType>(result.Errors)));

        return MoveToStateResult<OrderValidationResultType>.Ok;
    }
    
    private async Task<MoveToStateResult<OrderValidationResultType>> FromWaitingForRegistration_ToRegistrationSuccess(
        Order entity,
        CancellationToken token
    )
    {
        var result = entity.ChangeState(State.StateType.RegistrationSuccess, State.StatusType.Registered);
        if (result.IsError)
            return await Task.FromResult(
                new MoveToStateResult<OrderValidationResultType>(IsError: true,
                    new WorkflowProcessError<OrderValidationResultType>(result.Errors)));

        return MoveToStateResult<OrderValidationResultType>.Ok;
    }
    
    private async Task<MoveToStateResult<OrderValidationResultType>> FromWaitingForRegistration_ToRegistrationRejected(
        Order entity,
        CancellationToken token
    )
    {
        var result = entity.ChangeState(State.StateType.RegistrationRejected, State.StatusType.Editing);
        if (result.IsError)
            return await Task.FromResult(
                new MoveToStateResult<OrderValidationResultType>(IsError: true,
                    new WorkflowProcessError<OrderValidationResultType>(result.Errors)));

        return MoveToStateResult<OrderValidationResultType>.Ok;
    }
}