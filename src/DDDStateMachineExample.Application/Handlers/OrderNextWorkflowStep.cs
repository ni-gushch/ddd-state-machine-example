using DDDStateMachineExample.Application.Handlers.Common.Common;
using DDDStateMachineExample.Domain.OrderAggregate.Repositories;
using DDDStateMachineExample.Domain.OrderAggregate.Workflows;

namespace DDDStateMachineExample.Application.Handlers;

public interface IOrderWorkflowNextStepHandler
    : ICommandHandler<IOrderWorkflowNextStepHandler.Request, IOrderWorkflowNextStepHandler.Response>
{
    public record Request(long OrderId) : ICommand;

    public record Response();
}

public class OrderWorkflowNextStep(
    IOrderRepository orderRepository,
    OrderWorkflowFactory orderWorkflowFactory
) : IOrderWorkflowNextStepHandler
{
    public async Task<IOrderWorkflowNextStepHandler.Response> Handle(
        IOrderWorkflowNextStepHandler.Request request,
        CancellationToken token
    )
    {
        var order = await orderRepository.Get(request.OrderId, token);
        await order.ToNextState(orderWorkflowFactory, token);
        await orderRepository.Update(order, token);

        return new IOrderWorkflowNextStepHandler.Response();
    }
}