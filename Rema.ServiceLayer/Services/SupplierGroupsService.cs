using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rema.DbAccess;
using Rema.Infrastructure.Models;
using Rema.ServiceLayer.Interfaces;
using Serilog;

namespace Rema.ServiceLayer.Services
{
  public class SupplierGroupsService : ISupplierGroupsService
  {
    private DbSet<SupplierGroup> _set;
    private RpDbContext _context;
    public SupplierGroupsService(RpDbContext context)
    {
      this._set = context.SupplierGroups;
      this._context = context;
    }

    public async Task<IEnumerable<SupplierGroup>> GetSupplierGroups()
    {
      try
      {
        return await _set.ToListAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting suppliergroups");
        return null;
      }
    }

    public async Task<SupplierGroup> GetSupplierGroup(long id)
    {
      try
      {
        var supplierGroup = await _set.FindAsync(id);
        if (supplierGroup == null)
        {
          return null;
        }
        return supplierGroup;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting suppliergroup");
        return null;
      }
    }

    public async Task<bool> PutSupplierGroup(SupplierGroup supplierGroup)
    {
      try
      {
        _context.Entry(supplierGroup).State = EntityState.Modified;
        await _context.SaveChangesAsync();
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while saving changed suppliergroup");
        return false;
      }

      try
      {
        var affectedAllocations = await _context.Allocations.Where(s => s.HintsForSuppliers.Any(g => g.Group.Id == supplierGroup.Id)).ToListAsync();
        foreach (var afAl in affectedAllocations)
        {
          var copyHints = afAl.HintsForSuppliers;
          foreach (var hint in copyHints)
          {
            if (hint.Group.Id == supplierGroup.Id) hint.Group = supplierGroup;
          }
          afAl.HintsForSuppliers = copyHints;
          _context.Entry(afAl).State = EntityState.Modified;
        }
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while saving changed suppliergroups in allocation hints");
        return false;
      }
    }

    public async Task<bool> PostSupplierGroup(SupplierGroup supplierGroup)
    {
      try
      {
        _context.SupplierGroups.Add(supplierGroup);
        await _context.SaveChangesAsync();
        return true; // CreatedAtAction("GetSupplierGroup", new { id = supplierGroup.Id }, supplierGroup);
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while saving new suppliergroup");
        return false;
      }
    }


    public async Task<bool> DeleteSupplierGroup(long id)
    {
      SupplierGroup supplierGroup;

      try
      {
        supplierGroup = await _context.SupplierGroups.FindAsync(id);
        if (supplierGroup == null)
        {
          return false;
        }
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while getting suppliergroup");
        return false;
      }

      try
      {
        _context.SupplierGroups.Remove(supplierGroup);
        await _context.SaveChangesAsync();
        return true;
      }
      catch (Exception ex)
      {
        Log.Error(ex, "error while remoning suppliergrup");
        return false;
      }
    }
  }
}
