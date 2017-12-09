using Sitecore.Diagnostics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Sitecore.Modules.HtmlCacheBuilder
{
    public class Dispatcher : IDispatcher
    {
        public Dispatcher() { }

        private List<string> ListUrls { get; set; }

        public bool AsyncRequest { get; set; }

        public virtual void Dispatch(string[] urls)
        {
            ListUrls = urls.ToList();

            if (AsyncRequest)
                ThreadPool.QueueUserWorkItem(o => { PerformRequests(); });
            else
                PerformRequests();
        }

        protected virtual void PerformRequests()
        {
            foreach (var url in ListUrls)
            {
                using (var client = new HttpClient())
                {
                    var response = client.GetAsync(url).Result;

                    if (response.IsSuccessStatusCode)
                    {
                        var dummy = response.Content.ReadAsStringAsync().Result;
                    }

                    Log.Info(String.Format("HtmlCacheBuilder got status code '{0}' from '{1}'", response.StatusCode, url), this);
                }
            }
        }
    }
}
