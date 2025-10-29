using DB.Entities;
using MediatR;

namespace Auth;

// TODO: emit just UserEvent
// public abstract record UserEvent: INotification; doesn't work
// public class UserCreated(User user) : UserEvent(user) { } doesn't work too
public abstract record UserEvent;
public record UserCreated(User User, Dictionary<string, string>? Data) : UserEvent;
public record UserSignedIn(User User, Dictionary<string, string>? Data) : UserEvent;
public record UserEmailChanged(User User) : UserEvent;
public record UserLoginChanged(User User) : UserEvent;
public record UserPasswordChanged(User User) : UserEvent;
// TODO: use this
public record UserEmailConfirmed(User User) : UserEvent;
public record UserDeleted(User User) : UserEvent;

public class UserNotification(UserEvent @event) : INotification {
  public UserEvent Event { get; } = @event;
}