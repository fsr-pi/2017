using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Firma.Mvc.Models;
using Firma.Mvc.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Firma.Mvc.ViewModels;

namespace Firma.Mvc.Controllers
{
  public class MjestoController : Controller
  {
    private readonly FirmaContext ctx;
    private readonly AppSettings appData;
   
    public MjestoController(FirmaContext ctx, IOptions<AppSettings> options)
    {
      this.ctx = ctx;
      appData = options.Value;
    }

    public IActionResult Index(int page = 1, int sort = 1, bool ascending = true)
    {
      int pagesize = appData.PageSize;
      var query = ctx.Mjesto.AsNoTracking();
      int count = query.Count();

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

      System.Linq.Expressions.Expression<Func<Mjesto, object>> orderSelector = null;
      switch (sort)
      {
        case 1:
          orderSelector = m => m.PostBrMjesta;
          break;
        case 2:
          orderSelector = m => m.NazMjesta;
          break;
        case 3:
          orderSelector = m => m.PostNazMjesta;
          break;
        case 4:
          orderSelector = m => m.OznDrzaveNavigation.NazDrzave;
          break;
      }
      if (orderSelector != null)
      {
        query = ascending ?
               query.OrderBy(orderSelector) :
               query.OrderByDescending(orderSelector);
      }

      var mjesta = query
                  .Select(m => new MjestoViewModel
                  {
                    IdMjesta = m.IdMjesta,
                    NazivMjesta = m.NazMjesta,
                    PostBrojMjesta = m.PostBrMjesta,
                    PostNazivMjesta = m.PostNazMjesta,
                    NazivDrzave = m.OznDrzaveNavigation.NazDrzave
                  })
                  .Skip((page - 1) * pagesize)
                  .Take(pagesize)
                  .ToList();
      var model = new MjestaViewModel
      {
        Mjesta = mjesta,
        PagingInfo = pagingInfo
      };

      return View(model);
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
      var mjesto = ctx.Mjesto.Find(id);
      if (mjesto != null)
      {
        PrepareDropDownLists();
        return PartialView(mjesto);
      }
      else
      {
        return NotFound($"Neispravan id mjesta: {id}");
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(Mjesto mjesto)
    {
      if (mjesto == null)
      {
        return NotFound("Nema poslanih podataka");
      }
      bool checkId = ctx.Mjesto.Any(m => m.IdMjesta == mjesto.IdMjesta);
      if (!checkId)
      {
        return NotFound($"Neispravan id mjesta: {mjesto?.IdMjesta}");
      }

      PrepareDropDownLists();
      if (ModelState.IsValid)
      {
        try
        {
          ctx.Update(mjesto);
          ctx.SaveChanges();
          return StatusCode(302, Url.Action(nameof(Row), new { id = mjesto.IdMjesta }));
        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          return PartialView(mjesto);
        }
      }
      else
      {
        return PartialView(mjesto);
      }
    }

    public PartialViewResult Row(int id)
    {
      var mjesto = ctx.Mjesto
                       .AsNoTracking()
                       .Where(m => m.IdMjesta == id)
                       .Select(m => new MjestoViewModel
                       {
                         IdMjesta = m.IdMjesta,
                         NazivMjesta = m.NazMjesta,
                         PostBrojMjesta = m.PostBrMjesta,
                         PostNazivMjesta = m.PostNazMjesta,
                         NazivDrzave = m.OznDrzaveNavigation.NazDrzave
                       })
                       .SingleOrDefault();
      if (mjesto != null)
      {
        return PartialView(mjesto);
      }
      else
      {
        //vratiti prazan sadržaj?
        return PartialView("ErrorMessageRow", $"Neispravan id mjesta: {id}");
      }
    }

    [HttpGet]
    public IActionResult Create()
    {
      PrepareDropDownLists();
      return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(Mjesto mjesto)
    {
      if (ModelState.IsValid)
      {
        try
        {
          ctx.Add(mjesto);
          ctx.SaveChanges();

          TempData[Constants.Message] = $"Mjesto {mjesto.NazMjesta} dodano. Id mjesta = {mjesto.IdMjesta}";
          TempData[Constants.ErrorOccurred] = false;
          return RedirectToAction(nameof(Index));

        }
        catch (Exception exc)
        {
          ModelState.AddModelError(string.Empty, exc.CompleteExceptionMessage());
          PrepareDropDownLists();
          return View(mjesto);
        }
      }
      else
      {
        PrepareDropDownLists();
        return View(mjesto);
      }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
      var mjesto = ctx.Mjesto
                       .AsNoTracking()
                       .Where(m => m.IdMjesta == id)
                       .SingleOrDefault();
      if (mjesto != null)
      {
        try
        {
          string naziv = mjesto.NazMjesta;
          ctx.Remove(mjesto);
          ctx.SaveChanges();
          var result = new
          {
            message = $"Mjesto {naziv} sa šifrom {id} obrisano.",
            successful = true
          };
          return Json(result);
        }
        catch (Exception exc)
        {
          var result = new
          {
            message = "Pogreška prilikom brisanja mjesta: " + exc.CompleteExceptionMessage(),
            successful = false
          };
          return Json(result);
        }
      }
      else
      {
        return NotFound($"Mjesto sa šifrom {id} ne postoji");
      }
    }

    private void PrepareDropDownLists()
    {
      var hr = ctx.Drzava
                  .AsNoTracking()
                  .Where(d => d.OznDrzave == "HR")
                  .Select(d => new { d.NazDrzave, d.OznDrzave })
                  .FirstOrDefault();
      var drzave = ctx.Drzava
                      .AsNoTracking()
                      .Where(d => d.OznDrzave!= "HR")
                      .OrderBy(d => d.NazDrzave)
                      .Select(d => new { d.NazDrzave, d.OznDrzave })
                      .ToList();
      if (hr != null)
      {
        drzave.Insert(0, hr);
      }
      ViewBag.Drzave = new SelectList(drzave, nameof(Drzava.OznDrzave), nameof(Drzava.NazDrzave));
    }
  }
}
