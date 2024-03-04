using Microsoft.AspNetCore.Identity;

namespace MyRoboAdvisor.Domain.Entities;

public class ApplicationUser : IdentityUser
{
  public ApplicationUser()
  {
    Questionnaires = new HashSet<Questionnaire>();
  }

  public string FirstName { get; set; } = string.Empty;
  public string LastName { get; set; } = string.Empty;
  public ICollection<Questionnaire> Questionnaires { get; set; }
}