using System;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;

namespace FairfaxCounty.JCAS_Public_MVC.Helpers
{
    /// <summary>An HTML Helper that displays Prev/Next buttons and Page Number control for the specified paged list.</summary>
    public static class PagingLinksHelper
    {
        /// <summary>An HTML Helper that displays Prev/Next buttons and Page Number control for the specified paged list.</summary>
        /// <param name="html">The HTML helper instance that this method extends.</param>
        /// <param name="actionName">The name of the Action that displays the paged list.</param>
        /// <param name="controllerName">The name of the Controller that displays the paged list.</param>
        /// <param name="totalPages">The total number of pages in the paged list.</param>
        /// <returns>Prev/Next buttons and Page Number control for the specified paged list.</returns>
        public static MvcHtmlString PagingLinks(this HtmlHelper html, string actionName, string controllerName, int totalPages)
        {
            if (totalPages == 0)
            {
                return new MvcHtmlString(string.Empty);
            }
            else
            {
                var urlHelper = new UrlHelper(html.ViewContext.RequestContext);
                StringBuilder sbReturn = new StringBuilder();
                int page = 1;
                int pageSize = 25;
                try
                {
                    page = Convert.ToInt32(HttpContext.Current.Request.QueryString["page"] ?? "1");
                    pageSize = Convert.ToInt32(HttpContext.Current.Request.QueryString["pageSize"] ?? "25");
                }
                catch
                {
                }
                string orderBy = HttpContext.Current.Server.HtmlEncode(Convert.ToString(HttpContext.Current.Request.QueryString["orderBy"] ?? ""));
                string filter = HttpContext.Current.Server.HtmlEncode(Convert.ToString(HttpContext.Current.Request.QueryString["filter"] ?? ""));

                sbReturn.AppendLine(string.Format("<form action=\"{0}\" method=\"get\">", urlHelper.Action(actionName, controllerName)));
                sbReturn.AppendLine("<ul class=\"pager\">");
                if (page <= 1)
                {
                    sbReturn.AppendLine("<li id=\"desktopPrevious\" class=\"previous disabled\"><a href=\"#\">Prev</a></li>");
                }
                else
                {
                    sbReturn.AppendLine("<li id=\"desktopPrevious\" class=\"previous\">");
                    sbReturn.AppendLine(Convert.ToString(html.ActionLink("Prev", actionName, controllerName, new { page = (page - 1), pageSize = pageSize, orderBy = orderBy, filter = filter }, null)));
                    sbReturn.AppendLine("</li>");
                }
                sbReturn.AppendLine("<li><label for=\"page\">Page</label>");
                sbReturn.AppendFormat("<input type=\"text\" name=\"page\" id=\"page\" value=\"{0}\" style=\"width: 50px;\" /> of {1}", page, totalPages);
                sbReturn.AppendLine();
                sbReturn.AppendFormat("<input type=\"hidden\" name=\"pageSize\" id=\"pageSize\" value=\"{0}\" />", pageSize);
                sbReturn.AppendFormat("<input type=\"hidden\" name=\"orderBy\" id=\"orderBy\" value=\"{0}\" />", orderBy);
                sbReturn.AppendFormat("<input type=\"hidden\" name=\"filter\" id=\"filter\" value=\"{0}\" />", filter);
                sbReturn.AppendLine("<input type=\"submit\" value=\"Go\" class=\"btn btn-default\" />");
                sbReturn.AppendLine("</li>");

                if (page >= totalPages)
                {
                    sbReturn.AppendLine("<li id=\"desktopNext\" class=\"next disabled\"><a href=\"#\">Next</a></li>");
                }
                else
                {
                    sbReturn.AppendLine("<li id=\"desktopNext\" class=\"next\">");
                    sbReturn.AppendLine(Convert.ToString(html.ActionLink("Next", actionName, controllerName, new { page = (page + 1), pageSize = pageSize, orderBy = orderBy, filter = filter }, null)));
                    sbReturn.AppendLine("</li>");
                }
                sbReturn.AppendLine("</ul>");
                sbReturn.AppendLine("</form>");

                return new MvcHtmlString(sbReturn.ToString());
            }
        }
    }
}