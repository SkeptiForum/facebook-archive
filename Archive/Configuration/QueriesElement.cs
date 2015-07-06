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

namespace SkeptiForum.Archive.Configuration {

  /*============================================================================================================================
  | CLASS: QUERIES ELEMENT
  \---------------------------------------------------------------------------------------------------------------------------*/
  public class QueriesElement : ConfigurationElement {

    /*==========================================================================================================================
    | PROPERTY: GROUPS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    [ConfigurationProperty("groups", IsRequired = false)]
    public GroupQueryElement Groups {
      get {
        return (GroupQueryElement)this["groups"];
      }
      set {
        this["groups"] = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: POSTS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    [ConfigurationProperty("posts", IsRequired = false)]
    public QueryElement Posts {
      get {
        return (QueryElement)this["posts"];
      }
      set {
        this["posts"] = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: COMMENTS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    [ConfigurationProperty("comments", IsRequired = false)]
    public QueryElement Comments {
      get {
        return (QueryElement)this["comments"];
      }
      set {
        this["comments"] = value;
      }
    }

  } //Class
} //Namespace
