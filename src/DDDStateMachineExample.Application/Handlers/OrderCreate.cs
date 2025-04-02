using DDDStateMachineExample.Application.Handlers.Common.Common;
using DDDStateMachineExample.Domain.Common.Models;
using DDDStateMachineExample.Domain.OrderAggregate;
using DDDStateMachineExample.Domain.OrderAggregate.Repositories;

namespace DDDStateMachineExample.Application.Handlers;

public interface IOrderCreateHandler : ICommandHandler<IOrderCreateHandler.Request, IOrderCreateHandler.Response>
{
    public record Request(WorkflowId WorkflowId) : ICommand;

    public record Response(Order CreatedOrder);
}

internal class OrderCreate(IOrderRepository orderRepository): IOrderCreateHandler
{
    public async Task<IOrderCreateHandler.Response> Handle(IOrderCreateHandler.Request request, CancellationToken token)
    {
        var newOrder = Order.CreateNewInstance(request.WorkflowId);

        var result = await orderRepository.Create(newOrder, token);

        return new IOrderCreateHandler.Response(result);
    }
}