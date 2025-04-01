using System.Text;

namespace DDDStateMachineExample.Domain.Common.Models;

public class WorkflowProcessError<TEntityErrorType>
    where TEntityErrorType : Enum
{
    public TransitionError? TransitionError { get; }

    public TEntityErrorType[] EntityErrors { get; }

    public bool IsSuccess => TransitionError is null && EntityErrors.Length == 0;
    public bool IsTransitionError => TransitionError != null;
    public bool IsEntityErrors => EntityErrors.Length > 0;
    

    private WorkflowProcessError(TransitionError? transitionError, TEntityErrorType[] entityErrors)
    {
        TransitionError = transitionError;
        EntityErrors = entityErrors;
    }

    public WorkflowProcessError(TransitionError? transitionError) : this(transitionError, [])
    {
    }

    public WorkflowProcessError(TEntityErrorType[] entityErrors) : this(null, entityErrors)
    {
    }

    public static WorkflowProcessError<TEntityErrorType> TransitionNotFoundError(
        string fromState,
        string toState,
        string entityId
    ) =>
        new(new TransitionError(
                Message: $"Не найден переход из состояния {fromState} в состояние {toState} для сущности с id {entityId}",
                ErrorType: TransitionErrorType.TransitionNotFoundError
            )
        );

    public override string ToString()
    {
        var errorStringBuilder = new StringBuilder();
        if (TransitionError is not null)
            errorStringBuilder.Append($"Transition error - {TransitionError.ToString()}");
        if (EntityErrors.Length != 0)
            errorStringBuilder.AppendLine(
                $"Error while validate entity - {string.Join(", ", EntityErrors.Select(it => it.ToString()))}");
        return errorStringBuilder.ToString();
    }
}

/// <summary>
/// Ошибка осуществления перехода между состояниями.
/// </summary>
/// <param name="Message">Сообщение об ошибке.</param>
/// <param name="ErrorType">Тип ошибки.</param>
public record TransitionError(string Message, TransitionErrorType ErrorType)
{
    public override string ToString()
    {
        return $"Message: {Message}, ErrorType: {ErrorType.ToString()}";
    }
}

/// <summary>
/// Типы ошибок возникаемых при переходах.
/// </summary> 
public enum TransitionErrorType
{
    InternalError = 1,
    TransitionNotFoundError = 2,
}