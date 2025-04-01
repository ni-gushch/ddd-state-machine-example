namespace DDDStateMachineExample.Domain.OrderAggregate.Errors;

public class WorkflowNotFoundException : Exception
{
    public WorkflowNotFoundException(string message, Exception exception) : base(message, exception)
    {
    }

    public WorkflowNotFoundException(string message) : base(message)
    {
    }
}