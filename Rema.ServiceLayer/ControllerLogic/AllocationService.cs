using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Rema.DbAccess;
using Rema.Infrastructure.Models;
using PdfSharpCore.Drawing;
using PdfSharpCore.Drawing.Layout;
using PdfSharpCore.Pdf;
using Rema.Infrastructure.Email.Templates;
using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace Rema.ServiceLayer.ControllerLogic
{
  public interface IAllocationService
  {
    public Task<Allocation> FindAllocationAll(long id);
    public MemoryStream GenerateAllocationPrintPdf(Allocation allocation);
  }
  public class AllocationService : IAllocationService
  {
    public AllocationService(RpDbContext context)
    {
      this._context = context;
    }
    private readonly RpDbContext _context;
    public async Task<Allocation> FindAllocationAll(long id)
    {
      return await _context.Allocations
         .Include(o => o.Ressources)
         .Include(r => r.ReferencePerson)
         .Include(r => r.LastModifiedBy)
         .Include(r => r.CreatedBy)
         .Include(g => g.Gadgets).ThenInclude(g => g.SuppliedBy)
         .FirstOrDefaultAsync(i => i.Id == id);
    }

    public MemoryStream GenerateAllocationPrintPdf(Allocation allocation)
    {
      var template = new PrintTemplate(allocation);

      PdfDocument document = new PdfDocument();

      PdfPage page = document.AddPage();
      XGraphics gfx = XGraphics.FromPdfPage(page);
      XFont font = new XFont("Calibri", 12, XFontStyle.Bold);
      XTextFormatter tf = new XTextFormatter(gfx);

      XRect rect = new XRect(40, 50, page.Width * 0.8, page.Height * 7.5);
      gfx.DrawRectangle(XBrushes.White, rect);
      tf.DrawString(template.ToString(), font, XBrushes.Black, rect, XStringFormats.TopLeft);
      MemoryStream stream = new MemoryStream();
      document.Save(stream, false);

      return stream;
    }
  }
}

