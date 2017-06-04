using Microsoft.AspNetCore.Mvc;
using Firma.Mvc.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Microsoft.Extensions.Options;
using Firma.Mvc.ViewModels;
using System;
using Firma.Mvc.Extensions;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Firma.Mvc.Controllers
{
  public class DrzavaController : Controller
  {    
    private readonly FirmaContext ctx;
    private readonly AppSettings appData;
    private readonly ILogger logger;

    public DrzavaController(FirmaContext ctx, IOptions<AppSettings> options, ILogger<DrzavaController> logger)
    {
      this.ctx = ctx;
      appData = options.Value;
      this.logger = logger;
    }

    //jednostavna varijanta, bez straničenja i sortiranja
    //public IActionResult Index()
    //{
    //  var drzave = ctx.Drzava
    //                  .AsNoTracking()
    //                  .OrderBy(d => d.NazDrzave)
    //                  .ToList();
    //  return View("IndexSimple", drzave);
    //}

    public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
    {      
      int pagesize = appData.PageSize;

      var query = ctx.Drzava
                  .AsNoTracking();

      int count = query.Count();
      if (count == 0)
      {
        TempData[Constants.Message] = "Ne postoji niti jedna država.";
        TempData[Constants.ErrorOccurred] = false;
        return RedirectToAction(nameof(Create));
      }

      var pagingInfo = new PagingInfo
      {
        CurrentPage = page,
        Sort = sort,
        Ascending = ascending,
        ItemsPerPage = pagesize,
        TotalItems = count
      };
      if (page > pagingInfo.TotalPages)
      {
        return RedirectToAction(nameof(Index), new { page = pagingInfo.TotalPages, sort = sort, ascending = ascending });
      }

      System.Linq.Expressions.Expression<Func<Drzava, object>> orderSelector = null;
      switch (sort)
      {
        case 1:
          orderSelector = d => d.OznDrzave;
          break;
        case 2:
          orderSelector = d => d.NazDrzave;
          break;
        case 3:
          orderSelector = d => d.Iso3drzave;
          break;
        case 4:
          orderSelector = d => d.SifDrzave;
          break;
      }
      if (orderSelector != null)
      {
        query = ascending ?
               query.OrderBy(orderSelector) :
               query.OrderByDescending(orderSelector);
      }
      var drzave = query
                  .Skip((page - 1) * pagesize)
                  .Take(pagesize)
                  .ToList();
      var model = new DrzaveViewModel
      {
        Drzave = drzave,
        PagingInfo = pagingInfo
      };

      return View(model);
    }

    [HttpGet]
    public IActionResult Create()
    {
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Drzava drzava)
    {
      logger.LogTrace(JsonConvert.SerializeObject(drzava));
      if (ModelState.IsValid)
      {
        try
        {
          ctx.Add(drzava);
          ctx.SaveChanges();
          logger.LogInformation($"Država {drzava.NazDrzave} dodana.");

          TempData[Constants.Message] = $"Država {drzava.NazDrzave} dodana.";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index));
        }
        catch (Exception exc)
        {          
          logger.LogError("Pogreška prilikom dodavanje nove države: {0}", exc.CompleteExceptionMessage());          
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          return View(drzava);
        }
      }
      else
      {
        return View(drzava);
      }
    }

    [HttpGet]
    public IActionResult Edit(String id, int page = 1, int sort = 1, bool ascending = true)
    {
      var drzava = ctx.Drzava.AsNoTracking().Where(d => d.OznDrzave == id).SingleOrDefault();
      if (drzava == null)
      {
        logger.LogWarning("Ne postoji država s oznakom: {0} ", id);
        return NotFound("Ne postoji država s oznakom: " + id);
      }
      else
      {
        ViewBag.Page = page;
        ViewBag.Sort = sort;
        ViewBag.Ascending = ascending;
        return View(drzava);
      }
    }

    [HttpPost, ActionName("Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Update(string id, int page = 1, int sort = 1, bool ascending = true)
    {
      //za različite mogućnosti ažuriranja pogledati
      //attach, update, samo id, ...
      //https://docs.asp.net/en/latest/data/ef-mvc/crud.html#update-the-edit-page

      try
      {
        Drzava drzava = await ctx.Drzava
                          .Where(d => d.OznDrzave == id)
                          .FirstOrDefaultAsync();
        if (drzava == null)
        {
          return NotFound("Neispravna oznaka države: " + id);
        }        

        if (await TryUpdateModelAsync<Drzava>(drzava, "",
            d => d.NazDrzave, d => d.SifDrzave, d => d.Iso3drzave
        ))
        {
          ViewBag.Page = page;
          ViewBag.Sort = sort;
          ViewBag.Ascending = ascending;
          try
          {
            await ctx.SaveChangesAsync();
            TempData[Constants.Message] = "Država ažurirana.";
            TempData[Constants.ErrorOccurred] = false;
            return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
          }
          catch (Exception exc)
          {
            ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
            return View(drzava);
          }
        }
        else
        {
          ModelState.AddModelError(string.Empty, "Podatke o državi nije moguće povezati s forme");
          return View(drzava);
        }
      }
      catch (Exception exc)
      {
        TempData[Constants.Message] = exc.CompleteExceptionMessage();
        TempData[Constants.ErrorOccurred] = true;
        return RedirectToAction(nameof(Edit), id);
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(string OznDrzave, int page = 1, int sort = 1, bool ascending = true)
    {
      var drzava = ctx.Drzava
                       .AsNoTracking()
                       .Where(d => d.OznDrzave == OznDrzave)
                       .SingleOrDefault();
      if (drzava != null)
      {
        try
        {
          string naziv = drzava.NazDrzave;
          ctx.Remove(drzava);
          ctx.SaveChanges();
          TempData[Constants.Message] = $"Država {naziv} uspješno obrisana";
          TempData[Constants.ErrorOccurred] = false;
          logger.LogInformation($"Država {naziv} uspješno obrisana");
        }
        catch (Exception exc)
        {
          TempData[Constants.Message] = "Pogreška prilikom brisanja države: " + exc.CompleteExceptionMessage();
          logger.LogError("Pogreška prilikom brisanja države: " + exc.CompleteExceptionMessage());
          TempData[Constants.ErrorOccurred] = true;
        }
      }
      else
      {
        TempData[Constants.Message] = "Ne postoji država s oznakom: " + OznDrzave;
        TempData[Constants.ErrorOccurred] = true;
      }
      return RedirectToAction(nameof(Index), new { page = page, sort = sort, ascending = ascending });
    }
  }
}