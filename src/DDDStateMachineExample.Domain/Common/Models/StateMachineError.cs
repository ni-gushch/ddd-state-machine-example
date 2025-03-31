using System.Text;

namespace DDDStateMachineExample.Domain.Common.Models;

public class StateMachineError<TEntityErrorType>
    where TEntityErrorType : notnull
{
    public TransitionError? TransitionError { get; }
    
    public TEntityErrorType[] EntityErrors { get; }
    
    public bool IsTransitionError => TransitionError != null;

    private StateMachineError(TransitionError? transitionError, TEntityErrorType[] entityErrors)
    {
        TransitionError = transitionError;
        EntityErrors = entityErrors;
    }

    public StateMachineError(TransitionError? transitionError) : this(transitionError, [])
    {
    }

    public StateMachineError(TEntityErrorType[] entityErrors) : this (null, entityErrors)
    {
    }

    public override string ToString()
    {
        var errorStringBuilder = new StringBuilder();
        if(TransitionError is not null)
            errorStringBuilder.Append($"Transition error - {TransitionError.ToString()}");
        if(EntityErrors.Length != 0)
            errorStringBuilder.AppendLine($"Error while validate entity - {string.Join(", ", EntityErrors.Select(it => it.ToString()))}");
        return errorStringBuilder.ToString();
    }
}

/// <summary>
/// Ошибка осуществления перехода между состояниями.
/// </summary>
/// <param name="Message">Сообщение о ошибке.</param>
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