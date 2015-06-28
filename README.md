# Facebook Archive
The [Skepti-Forum Project](http://www.skeptiforum.org/) runs a collection of Facebook discussion groups dedicated to civil, evidence-based discussion of science related issues. The Facebook Archive provides a website for archiving public content and then displaying it to end users via a Microsoft ASP.NET MVC application.

## [Website Project](./Website)
The website frontend is a Microsoft ASP.NET MVC 5 project using Google's AngularJS for exposing the data to end users. This includes:
- `/` for searching the site via a Google Custom Search Engine (CSE).
- `/Admin/Download/` for requesting archives from Facebook using the [Archive class library](./Archive).
- `/Sitemap` for providing metadata to Google and Bing search engines (via the [sitemap.org](http://sitemap.org/) format).
- `/Groups/#/permalink/#` for exposing details for individual threads (via the `ThreadsController`).

## [Archive Class Library](./Archive)
The Archive class library provides a data access layer for interacting with the Facebook Graph API. This includes a static `Facebook` class for accessing a cached instance of a `FacebookClient` (via the `Client` property) and a `FacebookGroupCollection` (via the `Groups` property). Currently, the data that is downloaded is stored to disk in an `/Archives/` folder (excluded from the repository). Within the library, calls to Facebook's Graph API are handled by the [Facebook C# SDK](/facebook-csharp-sdk/facebook-csharp-sdk).
> *Note:* The Archive project contains a `Constants.cs` class which must be modified to include the Facebook API Key (via the `ClientId` property) and App Secret (via the `ClientSecret` property). If these are not set, the administration tools will throw a `NotImplementedException`.

## Download Tool
The download tool is required for establishing and maintaining the archive of Facebook posts. It can be called via the `Admin` controller (`/Admin/Download`), and largely relies on the [Archive class library](./Archive). Out-of-the-box, the `Archive` class library will download all posts from public Facebook groups that a) the authenticated user is a member of, and b) containing the term 'Skepti-Forum' in their title. The payloads for these requests are then saved by the `AdminController` to disk, in the format `/{GroupId}/{GroupId}_{PostId}.json`. It is these files that the frontend web application provides access to through the `/Groups/{GroupId}/permalink/{PostId}` route (which is consistent with Facebook's own routing conventions).
> *Note:* Facebook recently deprecated the `user_groups` permission for public apps, except in very specific scenarios. For now, the permission continues to work for unpublished apps. As a result, the download functionality is only accessible to users who are listed as an owner, developer, or tester of the Facebook application. 

### Privacy
By default, the download tool will only archive posts from groups that are marked as "Public". For groups marked as "Closed" or "Private", members may have an expectation of privacy, even if join requests are always approved. The tool can be modified by reinstantiating the `Facebook.Groups` property with an instance of `new FacebookGroupCollection(false);` which will override the `publicOnly` configuration element. This should only be done with a clear understanding of your user's privacy expectations. 
> *Note:* In the future, we will be extending this project to include an anonymizer routine which will replace name with initials. Until then, we strongly discourage downloading data from "Closed" or "Private" groups.

### Disclaimer
This tool was developed primarily as an internal application to meet the requirements of the Skepti-Forum Project and, as such, it has not been optimized for public consumption. There are a number of areas of the code that need cleanup, particularly relating to how Facebook access tokens are stored, sent to the server, and used to instantiate the `FacebookClient` class. While we hope to refactor this process in the future, be aware that the administrative tools have been developed with a "get 'er done" mentality, and not an eye toward reuse. YMMV.

## License
This application is released on the [MIT License](./LICENSE).
