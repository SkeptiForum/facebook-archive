# Facebook Archive
The Skepti-Forum Project runs a collection of Facebook discussion groups dedicated to civil, evidence-based discussion of science related issues. The Facebook Archive provides a website for archiving public content and then displaying it to end users via a Microsoft ASP.NET MVC application.

## [Archive Class Library](./Archive)
The Archive class library provides a data access layer for interacting with the Facebook Graph API. This includes a static `Facebook` class for accessing a cached instance of a `FacebookClient` (via the `Client` property) and a `FacebookGroupCollection` (via the `Groups` property). Currently, the data that is downloaded is stored to disk in an `/Archives/` folder (excluded from the repository). Within the library, calls to Facebook's Graph API are handled by the [Facebook C# SDK](/facebook-csharp-sdk/facebook-csharp-sdk).
> *Note:* The Archive project contains a `Constants.cs` class which must be modified to include the Facebook API Key (via the `ClientId` property) and App Secret (via the `ClientSecret` property). If these are not set, the administration tools will throw a `NotImplementedException`.

## [Website Project](./Website)
The website frontend is a Microsoft ASP.NET MVC 5 project using Google's AngularJS for exposing the data to end users. This includes:
- A Google Custom Search Engine (CSE) for the homepage.
- `/Admin/Download/` for requesting archives from Facebook.
- `/Sitemap` for providing metadata to Google and Bing search engines (via the [sitemap.org](http://sitemap.org/) format).
- `/Groups/#/permalink/#` for exposing details for individual threads (via the `ThreadsController`).
