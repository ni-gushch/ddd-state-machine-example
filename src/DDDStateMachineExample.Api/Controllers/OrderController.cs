using DDDStateMachineExample.Application.Handlers;
using Microsoft.AspNetCore.Mvc;

namespace DDDStateMachineExample.Api.Controllers;

[ApiController]
[Route("/api/order")]
public class OrderController : ControllerBase
{
    [HttpPost("state/next")]
    public async Task Next([FromQuery] long orderId,
        [FromServices] IOrderWorkflowNextStepHandler nextStepHandler,
        CancellationToken token)
    {
        await nextStepHandler.Handle(new IOrderWorkflowNextStepHandler.Request(orderId), token);
    }
    
    [HttpPost("state/previous")]
    public async Task Previous([FromQuery] long orderId,
        [FromServices]IOrderWorkflowPreviousStepHandler previousStepHandler,
        CancellationToken token)
    {
        await previousStepHandler.Handle(new IOrderWorkflowPreviousStepHandler.Request(orderId), token);
    }
}