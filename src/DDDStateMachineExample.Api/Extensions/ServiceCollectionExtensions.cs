using DDDStateMachineExample.Api.ExceptionHandlers;

namespace DDDStateMachineExample.Api.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddExceptionHandlers(this IServiceCollection services)
    {
        services
            .AddExceptionHandler<NotFoundExceptionHandler>();

        return services;
    }
}