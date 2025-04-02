namespace DDDStateMachineExample.Api.Models;

public record OrderCreateInputModel(long WorkflowId);

public record OrderCreateViewModel(long OrderId);