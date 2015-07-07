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
  /// <summary>
  ///   Provides an element for configuring defaults for queries to the <see 
  ///   cref="SkeptiForum.Archive.Providers.ArchiveDataProviderBase"/> via <see cref="QueryElement"/> properties. 
  /// </summary>
  public class QueriesElement : ConfigurationElement {

    /*==========================================================================================================================
    | PROPERTY: GROUPS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides configuration access to the <see cref="SkeptiForum.Archive.Providers.ArchiveStorageProviderBase"/> for 
    ///   querying groups information. 
    /// </summary>
    /// <remarks>
    ///   This element includes configuration for fields and limits, as well as group-specific properties such as whether 
    ///   or not to include non-public groups in the query results.
    /// </remarks>
    /// 
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
    ///   Provides configuration access to the <see cref="SkeptiForum.Archive.Providers.ArchiveStorageProviderBase"/> for 
    ///   querying posts information. 
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
    ///   Provides configuration access to the <see cref="SkeptiForum.Archive.Providers.ArchiveStorageProviderBase"/> for 
    ///   querying comments information. 
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
