using Auth;
using DB;
using DB.Entities;
using Market.ServiceExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Market.Profile.Handlers;


public class UserConsumer(AppDbContext db, ILogger<UserConsumer> logger, InvitationService invitationAcceptanceService) : INotificationHandler<UserNotification> {

    public async Task Handle(UserNotification userNotification, CancellationToken cancellationToken) {
        var userEvent = userNotification.Event;

        switch (userEvent) {
            case UserCreated userCreated: {
                    var profile = await db.Profiles.FirstOrDefaultAsync(p => p.Id == userCreated.User.Id, cancellationToken: cancellationToken);

                    if (profile != null) {
                        logger.LogDebug("User profile for {@UserId}({@UserEmail}) already exists", userCreated.User.Id, userCreated.User.Email);
                    } else {
                        profile = new DB.Entities.Profile() {
                            Id = userCreated.User.Id,
                            Email = userCreated.User.Email ?? throw new ArgumentException("UserEmail is null"),
                            Login = string.IsNullOrWhiteSpace(userCreated.User.Email)
                                ? throw new ArgumentException("UserEmail is empty")
                                : userCreated.User.Email.Split('@')[0],
                        };

                        if (userCreated.Data != null) {
                            // Update FKs 
                            // TODO Future data
                            new List<(string, Action<Guid?>)> {
                            }.ForEach((fk) => {
                                var value = userCreated.Data?.GetValueOrDefault(fk.Item1);
                                if (value == null) {
                                    return;
                                }
                                if (Guid.TryParse(value, out var guid)) {
                                    fk.Item2(guid);
                                } else {
                                    logger.LogWarning("Invalid {@Key} value for user {@UserId}: '{@Value}' is not a valid Guid", fk.Item1, userCreated.User.Id, value);
                                }
                            });

                            // Update name
                            var firstName = userCreated.Data.GetValueOrDefault("firstName");
                            if (!string.IsNullOrWhiteSpace(firstName)) {
                                profile.FirstName = firstName;
                            } else {
                                logger.LogWarning("Invalid FirstName for user {@UserId}: '{@Value}' is not a valid Guid", userCreated.User.Id, firstName);
                            }
                            var lastName = userCreated.Data.GetValueOrDefault("lastName");
                            if (!string.IsNullOrWhiteSpace(lastName)) {
                                profile.LastName = lastName;
                            } else {
                                logger.LogWarning("Invalid LastName for user {@UserId}: '{@Value}' is not a valid Guid", userCreated.User.Id, lastName);
                            }
                            db.Profiles.Add(profile);
                            await db.SaveChangesAsync(cancellationToken);
                            // Invite accounte link
                            if (userCreated.Data.TryGetValue("invitationId", out var invitationIdStr) &&
                                userCreated.Data.TryGetValue("token", out var token) &&
                                Guid.TryParse(invitationIdStr, out var invitationId)) {

                                var accepted = await invitationAcceptanceService.TryAcceptInvitationAsync(
                                    invitationId,
                                    token,
                                    profile,
                                    cancellationToken
                                );

                                if (!accepted) {
                                    logger.LogWarning("Invalid or expired invitation for user {@UserId}", userCreated.User.Id);
                                }
                            }

                        }
                        // var blankAccount = new Account {
                        //     Status = AccountStatus.Blank,
                        //     Type = AccountType.Regular,
                        //     Profiles = [profile],
                        // };
                        // db.Accounts.Add(blankAccount);
                    }

                    await db.SaveChangesAsync(cancellationToken);
                    logger.LogInformation("User profile for {@UserId}({@UserEmail}) was created", userCreated.User.Id, userCreated.User.Email);
                }
                break;

            case UserEmailChanged userEmailChanged: {
                    var profile = await db.Profiles.FirstOrDefaultAsync(p => p.Id == userEmailChanged.User.Id, cancellationToken) ??
                                            throw new NullReferenceException("Profile is null");

                    profile.Email = userEmailChanged.User.Email ??
                        throw new ArgumentException("UserEmail is null");
                    db.Profiles.Update(profile);
                    await db.SaveChangesAsync(cancellationToken);

                    logger.LogInformation("User {@UserId} email was changed: {@UserEmail}", userEmailChanged.User.Id, userEmailChanged.User.Email);
                }
                break;

            case UserDeleted userDeleted: {
                    var profile = await db.Profiles.FirstOrDefaultAsync(p => p.Id == userDeleted.User.Id, cancellationToken) ??
                        throw new NullReferenceException("Profile is null");

                    db.Profiles.Remove(profile);
                    await db.SaveChangesAsync(cancellationToken);
                    logger.LogInformation("Profile for UserId:{@UserId} was removed", userDeleted.User.Id);
                }
                break;
            case UserSignedIn userSignedIn: {
                    if (userSignedIn.Data != null) {
                        var profile = await db.Profiles.FirstOrDefaultAsync(q => q.Id == userSignedIn.User.Id, cancellationToken) ??
                        throw new NullReferenceException("Profile is null");
                        if (userSignedIn.Data.TryGetValue("invitationId", out var invitationIdStr) &&
                                        userSignedIn.Data.TryGetValue("token", out var token) &&
                                        Guid.TryParse(invitationIdStr, out var invitationId)) {

                            var accepted = await invitationAcceptanceService.TryAcceptInvitationAsync(
                                invitationId,
                                token,
                                profile,
                                cancellationToken
                            );
                            if (!accepted) {
                                logger.LogWarning("Invalid or expired invitation for user {@UserId}", userSignedIn.User.Id);
                            }
                        }
                    }
                }
                break;
            default:
                break;
        }
    }
}