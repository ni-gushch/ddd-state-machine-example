namespace DDDStateMachineExample.Domain.OrderAggregate.Errors;

public record struct OrderValidationResult(bool IsError, OrderValidationResultType[] Errors);

public enum OrderValidationResultType
{
    
}