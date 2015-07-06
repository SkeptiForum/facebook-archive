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
  public class ApiElement : ConfigurationElement {

    /*==========================================================================================================================
    | PROPERTY: APP KEY
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
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
    ///   
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
    ///   
    /// </summary>
    public string Token {
      get {
        return HttpContext.Current.Request.Cookies["fbat"].Value;
      }
      set {
        SetToken(value);
      }
    }

    public void SetToken(string accessToken) {
      SetToken(accessToken, DateTime.Now.AddMonths(1));
    }

    public void SetToken(string accessToken, long expiry) {
      SetToken(accessToken, DateTime.Now.AddSeconds(expiry));
    }

    public void SetToken(string accessToken, DateTime expiry) {
      var cookie = new HttpCookie("fbat", accessToken);
      cookie.Expires = expiry;
      HttpContext.Current.Response.Cookies.Add(cookie);
    }


  } //Class
} //Namespace
