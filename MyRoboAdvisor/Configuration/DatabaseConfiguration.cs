namespace MyRoboAdvisor.Configuration;

public record DatabaseConfiguration
{
  public string? ConnectionString { get; init; }
}
