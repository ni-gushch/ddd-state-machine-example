using DDDStateMachineExample.Api.Models;
using DDDStateMachineExample.Application.Handlers;
using DDDStateMachineExample.Domain.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace DDDStateMachineExample.Api.Controllers;

[ApiController]
[Route("/api/order")]
public class OrderController : ControllerBase
{
    [HttpPost()]
    public async Task<OrderCreateViewModel> Create([FromBody] OrderCreateInputModel input,
        [FromServices] IOrderCreateHandler handler,
        CancellationToken token)
    {
        var result = await handler.Handle(new IOrderCreateHandler.Request(new WorkflowId(input.WorkflowId)), token);

        return new OrderCreateViewModel(result.CreatedOrder.Id);
    }
    
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