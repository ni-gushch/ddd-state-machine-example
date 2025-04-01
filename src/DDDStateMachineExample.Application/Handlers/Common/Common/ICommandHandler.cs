namespace DDDStateMachineExample.Application.Handlers.Common.Common;

public interface ICommand
{
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler
    where TCommand : ICommand
{
    Task<TResponse> Handle(
        TCommand request,
        CancellationToken token
    );
}

public interface ICommandHandler<in TCommand> : IRequestHandler
    where TCommand : ICommand
{
    Task Handle(
        TCommand request,
        CancellationToken token
    );
}
