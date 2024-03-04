namespace MyRoboAdvisor.Domain.Entities;

public class Advice : BaseEntity
{
    public string Recommendation { get; set; } = string.Empty;
    public Guid QuestionnaireId { get; set; }
    public Questionnaire Questionnaire { get; set; }
}