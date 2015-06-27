/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Facebook;

namespace SkeptiForum.Archive {

  /*============================================================================================================================
  | CLASS: FACEBOOK
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides a static reference to persistent resources associated with Facebook. This includes a cached reference to the 
  ///   authenticated Facebook client, as instantiated by a calling client, as well as a cached list of groups.
  /// </summary>
  public static class Facebook {

    /*==========================================================================================================================
    | PRIVATE VARIABLES
    \-------------------------------------------------------------------------------------------------------------------------*/
    private static FacebookGroupCollection _groups;
    private static FacebookClient _client;

    /*==========================================================================================================================
    | PROPERTY: GROUPS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets a reference to a collection of <see cref="FacebookGroup" /> objects associated with the project. Automatically 
    ///   loads groups from the Facebook Graph API using the default settings.
    /// </summary>
    public static FacebookGroupCollection Groups {
      get {
        if (_groups == null) {
          _groups = new FacebookGroupCollection();
        }
        return _groups;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: CLIENT
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets a reference to a <see cref="FacebookClient" /> object that will be shared by all users. It is expected that 
    ///   client code will check to ensure the client is instantiated with a <see cref="FacebookClient.AccessToken" /> and 
    ///   <see cref="FacebookClient.AppSecret" /> before referencing; if these are not set, then it is the responsibility of 
    ///   the client to look these up and provide them to the class.
    /// </summary>
    public static FacebookClient Client {
      get {
        if (_client == null) {
          _client = new FacebookClient();
        }
        return _client;
      }
    }

    /*==========================================================================================================================
    | METHOD: GET LONG LIVED TOKEN
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   When a user authenticated using the Facebook JavaScript SDK, a short-lived (session) access token is returned. This is 
    ///   not sufficient for continued use of the application. To mitigate this, client code may request that the access token
    ///   be replaced with a long-term access token.
    /// </summary>
    /// <param name="accessToken">The short-lived (session) access token assigned by the Facebook JavaScript SDK.</param>
    public static string GetLongLivedToken(string accessToken) {
      int expiry;
      return GetLongLivedToken(accessToken, out expiry);
    }

    /*==========================================================================================================================
    | METHOD: GET LONG LIVED TOKEN
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   When a user authenticated using the Facebook JavaScript SDK, a short-lived (session) access token is returned. This is 
    ///   not sufficient for continued use of the application. To mitigate this, client code may request that the access token
    ///   be replaced with a long-term access token. Will additionally return the expiration time of the access token via an 
    ///   output parameter; this will <i>typically</i> be sixty days, but that is not guaranteed by the Facebook Graph API.
    /// </summary>
    /// <param name="accessToken">The short-lived (session) access token assigned by the Facebook JavaScript SDK.</param>
    /// <param name="expiry">The expiration time of the long-term access token returned.</param>
    public static string GetLongLivedToken(string accessToken, out int expiry) {
    dynamic result = Client.Get(
      "oauth/access_token",
      new {
        client_id = Constants.ClientId.ToString(),
        client_secret = Constants.ClientSecret,
        grant_type = "fb_exchange_token",
        fb_exchange_token = accessToken
      }
    );
    expiry = result.expires;
    return result.access_token;
  }


  } //Class
} //Namespace
