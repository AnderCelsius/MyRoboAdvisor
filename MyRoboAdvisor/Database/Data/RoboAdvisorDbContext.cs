using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MyRoboAdvisor.Domain.Entities;
using System.Reflection;

namespace MyRoboAdvisor.Database.Data;

public class RoboAdvisorDbContext : IdentityDbContext<ApplicationUser>
{
    public RoboAdvisorDbContext(DbContextOptions<RoboAdvisorDbContext> options)
        : base(options)
    {
    }

    public DbSet<Questionnaire> Questionnaires => Set<Questionnaire>();
    public DbSet<Advice> Advices => Set<Advice>();

    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var item in ChangeTracker.Entries<BaseEntity>())
        {
            switch (item.State)
            {
                case EntityState.Modified:
                    item.Entity.LastModified = DateTime.UtcNow;
                    break;
                case EntityState.Added:
                    item.Entity.CreatedAt = DateTime.UtcNow;
                    break;
            }
        }
        return await base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}