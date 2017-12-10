using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Sites;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Sitecore.Modules.HtmlCacheBuilder
{
    public class UrlProducer : IUrlProducer
    {
        public UrlProducer() { }

        private readonly List<SiteSettings> _sites = new List<SiteSettings>();
        private readonly ArrayList _includedUrls = new ArrayList();
        private readonly ArrayList _excludedUrls = new ArrayList();
        private readonly Dictionary<string, Database> databases = new Dictionary<string, Database>();

        public List<SiteSettings> Sites
        {
            get
            {
                return this._sites;
            }
        }

        public ArrayList IncludedUrls { get { return this._includedUrls; } }

        public ArrayList ExcludedUrls { get { return this._excludedUrls; } }

        public int SiteDepth { get; set; }

        public string DefaultScheme { get; set; }

        public void AddSite(System.Xml.XmlNode node)
        {
            var name = Sitecore.Xml.XmlUtil.GetAttribute("name", node);
            var forceHost = Sitecore.Xml.XmlUtil.GetAttribute("forceHost", node);

            this.Sites.Add(new SiteSettings(name, forceHost));
        }

        public IEnumerable<string> GetUrls()
        {
            List<string> listURLs = new List<string>();

            //Get from sites
            foreach (SiteSettings siteSetting in Sites)
            {
                try
                {
                    listURLs.AddRange(GetURLs(siteSetting));
                }
                catch (Exception ex)
                {
                    Log.Error("Error building the cache for '" + siteSetting.SiteName + "'", ex, this);
                }
            }

            //Add Included
            listURLs.AddRange(IncludedUrls.ToArray().Select(c => (string)c));

            //Eliminate duplicateds
            listURLs = listURLs.Distinct().ToList();

            //Remove ones added to exclude
            listURLs = ApplyExcluded(listURLs);

            return listURLs;
        }

        protected virtual List<string> ApplyExcluded(List<string> listURLs)
        {
            foreach (string pattern in ExcludedUrls.ToArray().Select(c => (string)c))
                listURLs.RemoveAll(c => Regex.IsMatch(c, pattern, RegexOptions.IgnoreCase));

            return listURLs;
        }

        protected virtual List<string> GetURLs(SiteSettings siteSettings)
        {
            if (!String.IsNullOrWhiteSpace(siteSettings.SiteName))
            {
                SiteContext site = Factory.GetSite(siteSettings.SiteName);
                if (site != null)
                {
                    var db = GetDatabase(site);
                    var home = GetSiteHomepage(site, db);

                    List<string> listURLs = new List<string>();
                    BuildListURLs(siteSettings, site, home, 1, ref listURLs);

                    return listURLs;
                }
            }

            return new List<string>();
        }

        protected virtual void BuildListURLs(SiteSettings siteSettings, SiteContext site, Item baseItem, int depth, ref List<string> listURLs)
        {
            //start recursivity 
            if (depth < SiteDepth)
            {
                foreach (Item item in baseItem.GetChildren())
                {
                    BuildListURLs(siteSettings, site, item, depth + 1, ref listURLs);
                }
            }

            //actually process and get URL
            string url = string.Empty;
            using (var context = new SiteContextSwitcher(site))
            {
                try
                {
                    UrlOptions options = new UrlOptions()
                    {
                        AlwaysIncludeServerUrl = true
                    };

                    url = LinkManager.GetItemUrl(baseItem, options);

                    if (url.StartsWith(@"://"))
                    {
                        if (site.Properties["scheme"] != null && site.Properties["scheme"].ToString() != "")
                            url = site.Properties["scheme"].ToString() + url;
                        else
                            url = DefaultScheme + url;
                    }

                    UriBuilder builder = new UriBuilder(url);

                    if (!String.IsNullOrWhiteSpace(siteSettings.Host))
                        builder.Host = siteSettings.Host;

                    url = builder.Uri.ToString();
                }

                catch
                {
                    url = string.Empty;
                    Log.Warn(string.Format("HtmlCacheBuilder could not resolve url to item '{0}' and site '{1}'", baseItem.Paths.FullPath, siteSettings.SiteName), this);
                }
            }

            if (!String.IsNullOrWhiteSpace(url))
                listURLs.Add(url);
        }

        protected virtual Item GetSiteHomepage(SiteContext site, Database db)
        {
            string path = site.Properties["rootPath"];
            path += (!site.Properties["rootPath"].EndsWith("/") && !site.Properties["startItem"].StartsWith("/")) ? "/" : "";
            path += site.Properties["startItem"];

            return db.GetItem(path);
        }

        protected virtual Database GetDatabase(SiteContext site)
        {
            string dbName = site.Properties["database"];

            if (!databases.ContainsKey(dbName))
            {
                databases.Add(dbName, Sitecore.Configuration.Factory.GetDatabase(dbName));
            }

            return databases[dbName];
        }

    }
}
