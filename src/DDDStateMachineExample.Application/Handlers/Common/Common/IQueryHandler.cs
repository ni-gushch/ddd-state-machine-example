namespace DDDStateMachineExample.Application.Handlers.Common.Common;

public interface IQuery
{
}

public interface IQueryHandler<in TQuery, TResponse> : IRequestHandler
    where TQuery : IQuery
{
    public Task<TResponse> Handle(
        TQuery request,
        CancellationToken token
    );
}

public interface IQueryHandler<in TQuery> : IRequestHandler
{
    public Task Handle(
        TQuery request,
        CancellationToken token
    );
}
