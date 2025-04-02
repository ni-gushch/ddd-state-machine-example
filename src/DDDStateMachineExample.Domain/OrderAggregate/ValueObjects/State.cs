using DDDStateMachineExample.Domain.Common.Abstractions;
using JustPlatform.Domain;

namespace DDDStateMachineExample.Domain.OrderAggregate.ValueObjects;

public record State(
    State.StateType Current,
    State.StatusType Status
) : RecordValueObject, IState<State.StateType>
{
    protected override IEnumerable<object?> GetEqualityComponents()
    {
        yield return Current;
        yield return Status;
    }

    public enum StateType
    {
        New = 1,
        EditPropertiesStepOne = 2,
        EditPropertiesStepTwo = 3,
        EditPropertiesStepThree = 4,
        WaitingForRegistration = 5,
        RegistrationSuccess = 6,
        RegistrationRejected = 7,
        Cancellation = 8,
        Cancelled = 9,
    }

    public enum StatusType
    {
        New = 1,
        Editing = 2,
        Registration = 3,
        Registered = 4,
        Cancellation = 5,
        Cancelled = 6,
    }
}