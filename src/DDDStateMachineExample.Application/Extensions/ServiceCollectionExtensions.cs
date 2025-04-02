using DDDStateMachineExample.Application.Handlers;
using DDDStateMachineExample.Domain.OrderAggregate.Workflows;
using Microsoft.Extensions.DependencyInjection;

namespace DDDStateMachineExample.Application.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddApplicationLayerServices(this IServiceCollection services)
    {
        return services
            .AddSingleton<OrderWorkflowFactory>()
            .AddHandlers();
    }

    private static IServiceCollection AddHandlers(this IServiceCollection services)
    {
        return services
            .AddScoped<IOrderCreateHandler, OrderCreate>()
            .AddScoped<IOrderWorkflowNextStepHandler, OrderWorkflowNextStep>()
            .AddScoped<IOrderWorkflowPreviousStepHandler, OrderWorkflowPreviousStep>()
            .AddScoped<IGetWorkflowByRuleHandler, GetWorkflowByRule>();
    }
}