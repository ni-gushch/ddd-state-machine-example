using DDDStateMachineExample.Api.Models;
using DDDStateMachineExample.Application.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace DDDStateMachineExample.Api.Controllers;

[ApiController]
[Route("/api/workflow")]
public class WorkflowController : ControllerBase
{
    [HttpPost("by-rule")]
    [ProducesResponseType(typeof(GetWorkflowByRuleViewModel), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<GetWorkflowByRuleViewModel> GetWorkflowByRule(
        [FromBody] GetWorkflowByRuleInputModel request,
        [FromServices] IGetWorkflowByRuleHandler getWorkflowByRuleHandler,
        CancellationToken token
    )
    {
        var rule = await getWorkflowByRuleHandler.Handle(
            new IGetWorkflowByRuleHandler.Request(
                request.FirstRuleParameter,
                request.SecondRuleParameter),
            token
        );

        return new GetWorkflowByRuleViewModel(rule.WorkflowId.Value);
    }
}