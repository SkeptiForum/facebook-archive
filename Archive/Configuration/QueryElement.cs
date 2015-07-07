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

namespace SkeptiForum.Archive.Configuration {

  /*============================================================================================================================
  | CLASS: QUERY ELEMENT
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides an element for storing configuration data used by the <see 
  ///   cref="SkeptiForum.Archive.Providers.ArchiveStorageProviderBase"/> when querying, for instance, groups, posts, or 
  ///   comments.
  /// </summary>
  public class QueryElement : ConfigurationElement {

    /*==========================================================================================================================
    | PROPERTY: LIMIT
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets the limit to use when querying the target endpoint. The limit represents the number of records to return.
    /// </summary>
    /// <remarks>
    ///   When querying Facebook, it is important to note that while there is no official maximum limit, there is a timeout
    ///   which will be triggered if too much data is requested. For this reason, it is preferable to set a smaller limit for 
    ///   posts, for instance, and instead return a larger limit for comments; otherwise, additional queries would need to be 
    ///   triggered for each post that contains than the specified limit for comments. The defaults for each limit are 
    ///   specified by the <see cref="SkeptiForum.Archive.Providers.ArchiveStorageProviderBase"/>.
    /// </remarks>
    [ConfigurationProperty("limit")]
    public int? Limit {
      get {
        return (int)this["limit"];
      }
      set {
        this["limit"] = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: FIELDS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets the fields that should be returned when querying the target endpoint. 
    /// </summary>
    /// <remarks>
    ///   The defaults fields are specified by the <see cref="SkeptiForum.Archive.Providers.ArchiveStorageProviderBase"/>.
    /// </remarks>
    [ConfigurationProperty("fields")]
    public string Fields {
      get {
        var fields = (String)this["fields"];
        if (String.IsNullOrWhiteSpace(fields)) {
          return null;
        }
        return fields;
      }
      set {
        this["fields"] = value;
      }
    }

  } //Class
} //Namespace
