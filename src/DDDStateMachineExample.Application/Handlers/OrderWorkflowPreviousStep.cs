using DDDStateMachineExample.Application.Handlers.Common.Common;
using DDDStateMachineExample.Domain.OrderAggregate.Repositories;
using DDDStateMachineExample.Domain.OrderAggregate.Workflows;

namespace DDDStateMachineExample.Application.Handlers;

public interface IOrderWorkflowPreviousStepHandler
    : ICommandHandler<IOrderWorkflowPreviousStepHandler.Request, IOrderWorkflowPreviousStepHandler.Response>
{
    public record Request(long OrderId) : ICommand;

    public record Response();
}

public class OrderWorkflowPreviousStep(
    IOrderRepository orderRepository,
    OrderWorkflowFactory orderWorkflowFactory
) : IOrderWorkflowPreviousStepHandler
{
    public async Task<IOrderWorkflowPreviousStepHandler.Response> Handle(
        IOrderWorkflowPreviousStepHandler.Request request,
        CancellationToken token
    )
    {
        var order = await orderRepository.Get(request.OrderId, token);
        await order.ToNextState(orderWorkflowFactory, token);
        await orderRepository.Update(order, token);

        return new IOrderWorkflowPreviousStepHandler.Response();
    }
}