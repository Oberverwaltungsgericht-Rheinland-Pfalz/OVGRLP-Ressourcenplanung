using System;
using Raumplanung.Infrastructure.Models;

namespace Raumplanung.Infrastructure.Email
{
  public class Workflow : IEmailWorkflow
  {
    public Workflow()
    {
    }

    public void TriggerTask(WorkflowDomain domain, string action, User triggerUser, string context = "", Ressource ressource = null, Allocation allocation = null, AllocationPurpose allocationPurpose = null)
    {
      // logging
      // decision
      switch (domain)
      {
        case WorkflowDomain.Allocation:

          break;
      }
    }

    public void TriggerTask(string domain, string action, string triggerUser, string context)
    {
      throw new NotImplementedException();
    }
  }

  public enum WorkflowDomain
  {
    Allocation, Gadget, Administration
  }

  public enum CRUDaction
  {
    save, edit, update, delete
  }
}
