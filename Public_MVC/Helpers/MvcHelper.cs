using log4net;
using System;
using System.IO;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Caching;
using System.Web.Mvc;

namespace FairfaxCounty.JCAS_Public_MVC.Helpers
{
    /// <summary>Helper used throughout the application.</summary>
    public static class MvcHelper
    {
        private static readonly ILog Log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>Render the include paths.</summary>
        /// <param name="helper">Identify this helper.</param>
        /// <param name="path">The path of the include files.</param>
        /// <param name="findReplace">List of key value pairs to find and replace.</param>
        /// <returns></returns>
        public static MvcHtmlString RenderInclude(this HtmlHelper helper, string path, IEnumerable<KeyValuePair<string, string>> findReplace = null)
        {
            MvcHtmlString result = (MvcHtmlString)HttpRuntime.Cache.Get(path);
            if (result != null) return result;

            if (path.StartsWith("http://") || path.StartsWith("https://") || path.StartsWith("//"))
            {
                string url = path;
                if (url.StartsWith("//")) // protocol relative path
                {
                    url = string.Format("{0}:{1}",
                        HttpContext.Current.Request.IsSecureConnection ? "https" : "http",
                        url
                    );
                }

                if (Log.IsDebugEnabled)
                {
                    Log.Debug("fetching url: " + url);
                }
                using (WebClient webClient = new WebClient())
                {
                    webClient.UseDefaultCredentials = true;
                    webClient.Credentials = CredentialCache.DefaultCredentials;
                    try
                    {
                        byte[] data = webClient.DownloadData(url);
                        string s = Encoding.UTF8.GetString(data);
                        if (s != null)
                        {
                            if (findReplace != null)
                            {
                                foreach (var kv in findReplace)
                                {
                                    s = Regex.Replace(s, kv.Key, kv.Value);
                                }
                            }
                            result = new MvcHtmlString(s);
                            HttpRuntime.Cache.Insert(path, result, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
                        }
                    }
                    catch (WebException e)
                    {
                        Log.Error(string.Format("error fetching url {0}: {1}", url, e.Message));
                    }
                }
            }
            else
            {
                string filePath = HttpContext.Current.Server.MapPath(path);
                if (Log.IsDebugEnabled)
                {
                    Log.Debug("reading file: " + filePath);
                }
                try
                {
                    using (var sr = new StreamReader(filePath))
                    {
                        var s = sr.ReadToEnd();
                        sr.Close();
                        if (s != null)
                        {
                            if (findReplace != null)
                            {
                                foreach (var kv in findReplace)
                                {
                                    s = Regex.Replace(s, kv.Key, kv.Value);
                                }
                            }
                            result = new MvcHtmlString(s);
                            HttpRuntime.Cache.Insert(path, result, null, DateTime.Now.AddMinutes(60), Cache.NoSlidingExpiration);
                        }
                    }
                }
                catch (Exception e)
                {
                    Log.Error(string.Format("error reading file {0}: {1}", filePath, e.Message));
                }
            }
            return result;
        }
    }
}
