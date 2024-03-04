using MyRoboAdvisor.Domain.Enums;

namespace MyRoboAdvisor.Domain.Entities;

public class Questionnaire : BaseEntity
{
    public int Age { get; set; }
    public InvestmentPurpose InvestmentPurpose { get; set; }
    public int InvestmentHorizon { get; set; }
    public string InvestmentKnowledge { get; set; } = string.Empty;
    public string RiskTolerance { get; set; } = string.Empty;
    public double Amount { get; set; }
    public string Currency { get; set; } = string.Empty;

    public string UserId { get; set; }
    public ApplicationUser User { get; set; }

    public Guid AdviceId { get; set; }
    public Advice Advice { get; set; }
}