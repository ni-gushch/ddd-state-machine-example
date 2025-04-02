using DDDStateMachineExample.Application.Exceptions;
using DDDStateMachineExample.Application.Handlers.Common.Common;
using DDDStateMachineExample.Domain.Common.Models;

namespace DDDStateMachineExample.Application.Handlers;

public interface IGetWorkflowByRuleHandler 
    : IQueryHandler<IGetWorkflowByRuleHandler.Request, IGetWorkflowByRuleHandler.Response>
{
    public record Request(long? FirstRuleParameter, long? SecondRuleParameter) : IQuery;

    public record Response(WorkflowId WorkflowId);
}

internal class GetWorkflowByRule : IGetWorkflowByRuleHandler
{
    public async Task<IGetWorkflowByRuleHandler.Response> Handle(IGetWorkflowByRuleHandler.Request request,
        CancellationToken token)
    {
        if (request.FirstRuleParameter is not null || request.SecondRuleParameter is not null)
            return await Task.FromResult(new IGetWorkflowByRuleHandler.Response(new WorkflowId(1)));
        if (request.FirstRuleParameter is not null || request.SecondRuleParameter is null)
            return new IGetWorkflowByRuleHandler.Response(new WorkflowId(2));
        if (request.FirstRuleParameter is null || request.SecondRuleParameter is not null)
            return new IGetWorkflowByRuleHandler.Response(new WorkflowId(3));

        throw new NotFoundException("Не верные параметры для пересчета идентификатора бизнес процесса.");
    }
}