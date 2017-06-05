using Firma.Mvc.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Options;
using System;
using System.Linq;

namespace Firma.Mvc.TagHelpers
{
  /// <summary>
  /// Tag helper za kreiranje vlastitih poveznica na stranice u rezultatu nekog upravljača
  /// Upotrebljava se kao atribut HTML oznake *div* 
  /// <example>
  /// Primjer upotrebe
  /// ```
  /// <div page-info="@Model.PagingInfo" page-action="Index" page-classes-enabled="true"
  ///      page-class="btn" page-class-normal="btn-default"
  ///      page-class-selected="btn-primary" class="btn-group pull-right">
  /// </div>
  /// ```
  /// U datoteku *_ViewImports.cshtml* potrebno dodati `@addTagHelper *, Microsoft.AspNetCore.Mvc.TagHelpers`  
  /// te u pogled uključiti vlastitu javascript datoteku *gotopage.js*
  /// </example>
  /// </summary>
  [HtmlTargetElement("div", Attributes = "page-info")]
  public class PageLinkTagHelper : TagHelper
  {

    private readonly IUrlHelperFactory urlHelperFactory;  
    private readonly AppSettings appData;
    public PageLinkTagHelper(IUrlHelperFactory helperFactory, IOptions<AppSettings> options)
    {      
      urlHelperFactory = helperFactory;
      appData = options.Value;
    }

    [ViewContext]
    [HtmlAttributeNotBound]
    public ViewContext ViewContext { get; set; }

    /// <summary>
    /// Serijalizirani string koji sadrži informacije o trenutnoj i ukupnom broju stranicu 
    /// </summary>
    public PagingInfo PageInfo { get; set; }

    /// <summary>
    /// Serijalizirani string kojim se prenose informacije o aktivnom filtriranju podataka
    /// </summary>
    public DokumentFilter PageFilter { get; set; }

    /// <summary>
    /// Akcija na koju poveznica treba voditi
    /// </summary>
    public string PageAction { get; set; }

    /// <summary>
    /// Označava koriste li se stilovi za formiranje poveznica
    /// </summary>
    public bool PageClassesEnabled { get; set; } = false;

    /// <summary>
    /// Zajednički stil za poveznice na određenu stranicu. 
    /// Stil trenutne stranice i stil ostalih stranica se može posebno navesti
    /// <see cref="PageClass"/>
    /// <see cref="PageClassNormal"/>
    /// </summary>
    public string PageClass { get; set; }

    /// <summary>
    /// Naziv CSS stila za brojeve stranica različite od trenutne
    /// <see cref="PageClass"/>
    /// <seealso cref="PageClassSelected"/>
    /// </summary>
    public string PageClassNormal { get; set; }

    /// <summary>
    /// Naziv CSS stila za trenutnu stranicu
    /// <see cref="PageClass"/>
    /// <seealso cref="PageClassNormal"/>
    /// </summary>
    public string PageClassSelected { get; set; }

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
      int offset = appData.PageOffset;
      TagBuilder result = new TagBuilder("div");

      if (PageInfo.CurrentPage - offset > 1)
      {
        var tag = BuildTagForPage(1, "1..");
        result.InnerHtml.AppendHtml(tag);
      }
      for (int i = Math.Max(1, PageInfo.CurrentPage - offset);
               i <= Math.Min(PageInfo.TotalPages, PageInfo.CurrentPage + offset);
               i++)
      {
        if (i != PageInfo.CurrentPage)
        {
          var tag = BuildTagForPage(i);
          result.InnerHtml.AppendHtml(tag);
        }
        else
        {
          var tag = BuildPageInputTag(i.ToString());
          result.InnerHtml.AppendHtml(tag);
        }
      }

      if (PageInfo.CurrentPage + offset < PageInfo.TotalPages)
      {
        var tag = BuildTagForPage(PageInfo.TotalPages, ".. " + PageInfo.TotalPages);
        result.InnerHtml.AppendHtml(tag);
      }

      output.Content.AppendHtml(result.InnerHtml);     
    }


    /// <summary>
    /// Stvara polje za prikaz trenutne stranice i unos željene stranice
    /// </summary>
    /// <param name="text">Broj trenutne stranice</param>
    /// <returns></returns>
    private TagBuilder BuildPageInputTag(string text)
    {
      IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
      TagBuilder tag = new TagBuilder("input");
      tag.MergeAttribute("type", "text");
      tag.Attributes["value"] = text;
      tag.Attributes["data-current"] = text;
      tag.Attributes["data-min"] = "1";
      tag.Attributes["data-max"] = PageInfo.TotalPages.ToString();
      tag.Attributes["data-url"] = urlHelper.Action(PageAction, new
      {
        page = -1,
        sort = PageInfo.Sort,
        ascending = PageInfo.Ascending,
        filter = PageFilter?.ToString()
      });
      tag.AddCssClass("pagebox");//da ga se može pronaći
      if (PageClassesEnabled)
      {
        tag.AddCssClass(PageClass);
        tag.AddCssClass(PageClassSelected);
      }
      return tag;
    }

    /// <summary>
    /// Stvara oznaku za i-tu stranicu koristeći *i* kao sadržaj poveznice
    /// <seealso cref="BuildTagForPage(int, string)"/>
    /// </summary>
    /// <param name="i">broj stranice</param>
    /// <returns>TagBuilder s pripremljenom poveznicom</returns>
    private TagBuilder BuildTagForPage(int i)
    {
      return BuildTagForPage(i, i.ToString());
    }

    /// <summary>
    ///  Stvara oznaku za i-tu stranicu koristeći argument text kao sadržaj poveznice
    /// </summary>
    /// <param name="i">broj stranice</param>
    /// <param name="text">tekst poveznice</param>
    /// <returns>TagBuilder s pripremljenom poveznicom</returns>
    private TagBuilder BuildTagForPage(int i, string text)
    {
      IUrlHelper urlHelper = urlHelperFactory.GetUrlHelper(ViewContext);
      TagBuilder tag = new TagBuilder("a");
      tag.Attributes["href"] = urlHelper.Action(PageAction, new
      {
        page = i,
        sort = PageInfo.Sort,
        ascending = PageInfo.Ascending,
        filter = PageFilter?.ToString()
      });
      if (PageClassesEnabled)
      {
        tag.AddCssClass(PageClass);
        //tag.AddCssClass(i == PageInfo.CurrentPage ? PageClassSelected : PageClassNormal);
        tag.AddCssClass(PageClassNormal);
      }
      tag.InnerHtml.Append(text);
      return tag;
    }
  }
}
