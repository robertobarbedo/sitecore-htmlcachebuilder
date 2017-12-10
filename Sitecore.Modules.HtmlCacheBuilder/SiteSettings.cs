using System;
using System.Collections;
using System.Collections.Generic;

namespace Sitecore.Modules.HtmlCacheBuilder
{
    public class SiteSettings
    {
        public string SiteName { get; private set; }
        public string Host { get; private set; }

        public SiteSettings(string siteName, string host)
        {
            SiteName = siteName;
            Host = host.Replace("http://", "").Replace("https://", ""); 
        }

        public override string ToString()
        {
            return SiteName;
        }
    }
}
