using System;
using System.Collections.Generic;
using System.Text;

namespace Infrastructure.Email
{
    public interface IEmailWorkflow
    {
        void TriggerTask(string domain, string action, string triggerUser, string context);
        
    }
}
