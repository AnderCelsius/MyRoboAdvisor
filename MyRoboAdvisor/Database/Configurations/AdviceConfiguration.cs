using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyRoboAdvisor.Domain.Entities;

namespace MyRoboAdvisor.Database.Configurations;

public class AdviceConfiguration : IEntityTypeConfiguration<Advice>
{
    public void Configure(EntityTypeBuilder<Advice> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(a => a.Recommendation)
            .IsRequired();

        builder.HasOne(a => a.Questionnaire)
            .WithOne(q => q.Advice)
            .HasForeignKey<Advice>(a => a.Id);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.LastModified);
    }
}