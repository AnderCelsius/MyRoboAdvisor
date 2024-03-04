using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MyRoboAdvisor.Domain.Entities;
using MyRoboAdvisor.Domain.Enums;

namespace MyRoboAdvisor.Database.Configurations;

public class QuestionnaireConfiguration : IEntityTypeConfiguration<Questionnaire>
{
    public void Configure(EntityTypeBuilder<Questionnaire> builder)
    {
        builder.HasKey(a => a.Id);
        builder.Property(q => q.InvestmentKnowledge).IsRequired();
        builder.Property(q => q.RiskTolerance).IsRequired();
        builder.Property(q => q.Currency).IsRequired();

        builder.Property(q => q.InvestmentPurpose)
            .HasConversion(
                v => v.ToString(),
                v => (InvestmentPurpose)Enum.Parse(typeof(InvestmentPurpose), v));

        builder.HasOne(q => q.User)
            .WithMany(u => u.Questionnaires)
            .HasForeignKey(q => q.UserId);

        builder.Property(u => u.CreatedAt)
            .IsRequired();

        builder.Property(u => u.LastModified);
    }
}