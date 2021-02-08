using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Quartz;
using Rema.DbAccess;
using Rema.Infrastructure.Email;
using Rema.Infrastructure.Email.Templates;
using Rema.Infrastructure.Models;
using Serilog;

namespace Rema.ServiceLayer.Jobs
{
  public interface IRemindJob : IJob { }

  public class RemindJob : IRemindJob
  {
    protected readonly RpDbContext _context;
//    protected readonly IMapper _mapper;
    protected readonly IEmailTrigger _emailTrigger;
    private readonly IConfiguration _configuration;

    public RemindJob(RpDbContext context, IEmailTrigger emailTrigger, IConfiguration configuration)
    {
      this._context = context;
      this._emailTrigger = emailTrigger;
      this._configuration = configuration;
    }

    public async Task Execute(IJobExecutionContext context)
    {
      int remindRange = this._configuration.GetValue<int>("Reminder");
      var timeRange = DateTime.Now.AddDays(remindRange);
      var timeRangeDistance = DateTime.Now.AddDays(3);
      var timeRangeCreation = DateTime.Now.AddDays(-20);

      IList<Allocation> unreminded = new List<Allocation>();
      try
      {
       unreminded = await _context.Allocations
      .Include(o => o.Ressources)
      .Include(r => r.ReferencePerson)
      .Include(r => r.LastModifiedBy)
      .Include(r => r.CreatedBy)
      .Include(g => g.AllocationGadgets).ThenInclude(ag => ag.Gadget).ThenInclude(g => g.SuppliedBy)
      .Where(a => a
        .ReferencePerson != null && 

        // Termin muss weniger als 10 Tage entfernt sein
        a.From < timeRange &&

        // Termin muss noch mindestens 3 Tage entfernt sein
        a.From > timeRangeDistance &&
        
        // Terminerstellung muss mindestens 20 Tage her sein 
        a.CreatedAt < timeRangeCreation
        
        // wurde noch nicht erinnert
        && !a.Reminded 
        )
      .ToListAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting allocations");
        return;
      }

      foreach(var remindAllocation in unreminded)
      {
        var template = new RemindTemplate(remindAllocation);
        bool reminded = _emailTrigger.SendEmail(template, null, new List<string>(){ remindAllocation.ReferencePerson.Email });

        remindAllocation.Reminded = reminded;
      }

      try
      {
        await _context.SaveChangesAsync();
      } catch (Exception ex)
      {
        Log.Error(ex, "error while saving reminded allocations");
      }
    }
  }
}
