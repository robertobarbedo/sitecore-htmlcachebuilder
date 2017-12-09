using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Sitecore.Modules.HtmlCacheBuilder
{
    public interface IUrlProducer
    {
        IEnumerable<string> GetUrls();
    }
}
