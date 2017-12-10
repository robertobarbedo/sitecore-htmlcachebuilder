Sitecore.Modules.HtmlCacheRebuilder
-----------------------------------

This module will allow you to trigger web requests to your Sitecore website in order to rebuild some of the HTML caches cleared by a publishing.

It is made of two pieces, a DLL and a include configuration file.

You can find the usage by looking at the pieces in the configuration file.

It contains a handler that can be hooked in the publish:end event, and publish:end:remote for distributed delivery server.

The handler is made of two parts.

#Dispatcher# 

A class that will perform the web requests. It accepts one boolean parameter "AsyncRequest". Specially when using the publish:end event, the async request will give a better user experience on publishing.

#UrlCollectior#

This class will return a list of URLs to be called.

##Parameters##

Sites

Collection that you can add websites which you want to rebuild caches for. Each site element accepts two properties

name = The site name and in your Sitecore sites configuration
forceHost = To tell the module which host you want to build the URL. Useful in cases where you got a load balancer and you want to ensure that the request is made from an internal binding definition.

The urls will be built using Sitecore LinkManager on the scope of the site specified.

SiteDepth

A number that tells the module how deep in the tree it navigates to get URLS. 1 is for the homepage, 2 is for the homepage and all its immediate descendants. You can assign any number here.

DefaultScheme

Only in cases where the LinkManager cannot resolve the scheme, the module will use the value specified here.

IncludedUrls

A list of arbitrary URLs you want to call. You can only use the included URLs or you can combine the URLs from the Sites parameter above.

In cases where you have very specific pages what you want to rebuild the cache for, you can just add the URL to this list.

ExcludedUrls

Will be applied at the end and will remove any entry previouly calculated or informed in the IncludedUrls.

You can pass a raw URL or a Regex pattern to remove any URL that matches it.

For instance you can remove the 404 page by using <pattern>^.*\/404$</pattern> or informing the URL <url>http://mysite/404</url>
