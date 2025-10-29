using Db.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Db.Interceptors {
    public class DateInterceptor : SaveChangesInterceptor {
        public override ValueTask<InterceptionResult<int>> SavingChangesAsync(DbContextEventData eventData, 
            InterceptionResult<int> result, CancellationToken cancellationToken = default) {

            var dbContext = eventData.Context;
            if (dbContext is null) {
                return base.SavingChangesAsync(eventData, result, cancellationToken);
            }
            
            var entries = dbContext.ChangeTracker.Entries<IEntity>();
            foreach (var entry in entries) {
                if (entry.State == EntityState.Added) {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                } else if (entry.State == EntityState.Modified) {
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                }
            }
            return base.SavingChangesAsync(eventData, result, cancellationToken);
        }
    }
}
