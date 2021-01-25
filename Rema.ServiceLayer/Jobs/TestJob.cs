using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Quartz;

namespace Rema.ServiceLayer.Jobs
{
  public class TestJob : IJob
  {
    public static IList<string> log = new List<string>();
    public Task Execute(IJobExecutionContext context)
    {
      var now = DateTime.Now;
      log.Add("run:" + now.ToShortTimeString());
      return Task.CompletedTask;
    }
  }
}
