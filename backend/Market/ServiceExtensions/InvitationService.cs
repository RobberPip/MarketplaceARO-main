using System.Globalization;
using DB;
using DB.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Market.ServiceExtensions {
    public class InvitationService {
        private readonly AppDbContext _db;
        private readonly IDataProtector _protector;
        private readonly ILogger<InvitationService> _logger;
        private const int TokenLifetimeDays = 7;

        public InvitationService(
            IDataProtectionProvider dataProtectionProvider,
            AppDbContext db,
            ILogger<InvitationService> logger) {
            _protector = dataProtectionProvider.CreateProtector("InvitationToken");
            _db = db;
            _logger = logger;
        }

        public string GenerateToken(string email) {
            var payload = $"{email}|{DateTime.UtcNow.AddDays(TokenLifetimeDays):O}";
            return _protector.Protect(payload);
        }

        public bool ValidateToken(string token, string email) {
            try {
                var payload = _protector.Unprotect(token);
                var parts = payload.Split('|');
                if (parts.Length != 2)
                    return false;

                var tokenEmail = parts[0];
                var expires = DateTime.Parse(parts[1], null, DateTimeStyles.RoundtripKind);

                return tokenEmail.Equals(email, StringComparison.OrdinalIgnoreCase) && expires > DateTime.UtcNow;
            } catch {
                return false;
            }
        }


        public async Task<bool> TryAcceptInvitationAsync(Guid invitationId, string token, DB.Entities.Profile currentProfile, CancellationToken cancellationToken = default) {
            var invitation = await _db.Invitations
                .Include(i => i.Account)
                .FirstOrDefaultAsync(i => i.Id == invitationId, cancellationToken);

            if (invitation == null ||
                string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(invitation.Email) ||
                !string.Equals(currentProfile.Email, invitation.Email, StringComparison.OrdinalIgnoreCase) ||
                invitation.Status == InvitationStatus.Accepted)
                return false;

            var isValid = ValidateToken(token, invitation.Email);
            if (!isValid)
                return false;

            invitation.ProfileId = currentProfile.Id;
            invitation.Status = InvitationStatus.Accepted;
            invitation.AcceptedAt = DateTime.UtcNow;

            _db.Invitations.Update(invitation);
            await _db.SaveChangesAsync(cancellationToken);

            _logger.LogInformation("User {@UserId} accepted invitation {@InvitationId}", currentProfile.Id, invitation.Id);
            return true;
        }
        /// <summary>
        /// Очистка просроченных приглашений
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        public async Task CleanupExpiredInvitationsAsync(Guid accountId) {
            var now = DateTime.UtcNow;

            var expiredProfiles = await _db.Invitations
                .Where(ap => ap.AccountId == accountId && ap.AcceptedAt == null)
                .Where(ap => ap.CreatedAt.AddDays(TokenLifetimeDays) < now)
                .ToListAsync();

            if (expiredProfiles.Any()) {
                _db.Invitations.RemoveRange(expiredProfiles);
                await _db.SaveChangesAsync();
            }
        }
    }
}
