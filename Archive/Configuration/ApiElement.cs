/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using Facebook;
using System.Web;

namespace SkeptiForum.Archive.Configuration {

  /*============================================================================================================================
  | CLASS: API ELEMENT
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides configuration access to API information, such as API keys and secrets.
  /// </summary>
  /// <remarks>
  ///   This class additionally includes a convenience property for accessing tokens (which are not stored in the configuration, 
  ///   but rather in cookies).
  /// </remarks>
  public class ApiElement : ConfigurationElement {

    /*==========================================================================================================================
    | PROPERTY: APP KEY
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets the API key for the service.
    /// </summary>
    [ConfigurationProperty("key", IsRequired = true)]
    public string AppKey {
      get {
        return (String)this["key"];
      }
      set {
        this["key"] = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: APP SECRET
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets the API secret for the service. 
    /// </summary>
    [ConfigurationProperty("secret", IsRequired = true)]
    public string AppSecret {
      get {
        return (String)this["secret"];
      }
      set {
        this["secret"] = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: TOKEN
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets the token via the cookie.
    /// </summary>
    /// <remarks>
    ///   The token is not part of the configuration. Since it is tightly related to the API information, however, this property
    ///   is exposed as a convenience so it can easily be accessed in a similar way to other API-related properties.
    /// </remarks>
    public string Token {
      get {
        return HttpContext.Current.Request.Cookies["fbat"].Value;
      }
      set {
        SetToken(value);
      }
    }

    /*==========================================================================================================================
    | METHOD: SET TOKEN
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Sets the token with the default expiration date.
    /// </summary>
    /// <param name="accessToken">The user's access token.</param>
    public void SetToken(string accessToken) {
      SetToken(accessToken, DateTime.Now.AddMonths(1));
    }

    /*==========================================================================================================================
    | METHOD: SET TOKEN
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Sets the token with a user-defined expiration.
    /// </summary>
    /// <param name="accessToken">The user's access token.</param>
    /// <param name="expiry">The time (in seconds) in which the token should expire.</param>
    public void SetToken(string accessToken, long expiry) {
      SetToken(accessToken, DateTime.Now.AddSeconds(expiry));
    }

    /*==========================================================================================================================
    | METHOD: SET TOKEN
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Sets the token with a user-defined expiration date.
    /// </summary>
    /// <param name="accessToken">The user's access token.</param>
    /// <param name="expiry">The time at which the token should expire.</param>
    public void SetToken(string accessToken, DateTime expiry) {
      var cookie = new HttpCookie("fbat", accessToken);
      cookie.Expires = expiry;
      HttpContext.Current.Response.Cookies.Add(cookie);
    }


  } //Class
} //Namespace
