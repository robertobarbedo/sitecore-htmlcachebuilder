using Sitecore.Caching;
using Sitecore.Caching.Generics;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Links;
using Sitecore.Sites;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using Sitecore.Jobs;
using System.Text.RegularExpressions;

namespace Sitecore.Modules.HtmlCacheBuilder
{
    public class HtmlCacheBuilderHandler
    {
        public IDispatcher Dispatcher { get; set; }

        public IUrlProducer UrlProducer { get; set; }

        public HtmlCacheBuilderHandler()
        {
        }

        public void BuildCache(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");

            Assert.ArgumentNotNull(Dispatcher, "Dispatcher");
            Assert.ArgumentNotNull(UrlProducer, "UrlProducer");

            Log.Info("HtmlCacheBuilder rebuilding HTML caches has started.", this);

            var listURLs = UrlProducer.GetUrls();

            Log.Info("HtmlCacheBuilder " + listURLs.Count().ToString() + " URLs has been dispatched for web requesting.", this);

            Dispatcher.Dispatch(listURLs.ToArray());

            if (listURLs.Count() > 0)
                Dialog("Warm up has been trigerred.");

            Log.Info("HtmlCacheBuilder done.", this);

        }

        public void ShowAuthorMessage(object sender, EventArgs args)
        {
            Dialog("Warm up has been trigerred on the Delivery Server(s).");
        }

        protected virtual void Dialog(string message)
        {
            var publishJobs = Sitecore.Jobs.JobManager
                    .GetJobs().Where(x => x.Category.Equals("publish"))
                    .ToList();

            foreach (Job j in publishJobs)
            {
                if (j.Handle.IsLocal)
                {
                    j.Status.Messages.Add(message);
                }

            }
        }
    }
}