namespace DDDStateMachineExample.Api.Models;

public record GetWorkflowByRuleInputModel(long? FirstRuleParameter, long? SecondRuleParameter);

public record GetWorkflowByRuleViewModel(long WorkflowId);