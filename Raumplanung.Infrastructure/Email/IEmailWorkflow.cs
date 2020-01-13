namespace Raumplanung.Infrastructure.Email
{
  public interface IEmailWorkflow
  {
    void TriggerTask(string domain, string action, string triggerUser, string context);
  }
}
