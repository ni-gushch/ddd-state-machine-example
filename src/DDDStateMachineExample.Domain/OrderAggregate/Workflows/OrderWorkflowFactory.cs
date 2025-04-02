using DDDStateMachineExample.Domain.Common.Abstractions;
using DDDStateMachineExample.Domain.Common.Models;
using DDDStateMachineExample.Domain.OrderAggregate.Errors;
using DDDStateMachineExample.Domain.OrderAggregate.ValueObjects;

namespace DDDStateMachineExample.Domain.OrderAggregate.Workflows;

public class OrderWorkflowFactory
{
    public IWorkflow<Order, long, State.StateType, OrderValidationResultType> GetWorkflow(
        WorkflowId workflowId
    )
    {
        return workflowId.Value switch
        {
            1 => new FirstOrderWorkflowProcess(),
            2 => new SecondOrderWorkflowProcess(),
            _ => throw new WorkflowNotFoundException(
                $"Для бизнес процесса с идентификатором {workflowId.Value} не найдена реализация."
            ),
        };
    }
}
