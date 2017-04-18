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

    public PagingInfo PageInfo { get; set; }
    public DokumentFilter PageFilter { get; set; }

    public string PageAction { get; set; }
    public bool PageClassesEnabled { get; set; } = false;
    public string PageClass { get; set; }
    public string PageClassNormal { get; set; }
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

    private TagBuilder BuildTagForPage(int i)
    {
      return BuildTagForPage(i, i.ToString());
    }

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
