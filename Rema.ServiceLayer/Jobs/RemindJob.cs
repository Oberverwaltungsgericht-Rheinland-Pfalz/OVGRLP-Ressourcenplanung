using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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

    public RemindJob(RpDbContext context, IEmailTrigger emailTrigger)
    {
      this._context = context;
      this._emailTrigger = emailTrigger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
      var timeRange = DateTime.Now.AddDays(10);
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
        a.From > DateTime.Now && 
        a.From < timeRange
        && !a.Reminded // was reminded boolean
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
        this._emailTrigger.SendEmail(template, null, new List<string>(){ remindAllocation.ReferencePerson.Email });

        remindAllocation.Reminded = true;
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
