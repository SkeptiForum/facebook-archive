/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SkeptiForum.Archive.Configuration {

  /*============================================================================================================================
  | CLASS: GROUP QUERY ELEMENT
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides a specialized element for configuring query information for groups, specifically. 
  /// </summary>
  /// <remarks>
  ///   In addition to the properties inherited from <see cref="QueryElement"/>, the <see cref="GroupQueryElement"/> supports
  ///   definition of group-specific properties such as <see cref="PublicOnly"/>.
  /// </remarks>
  public class GroupQueryElement : QueryElement {

    /*==========================================================================================================================
    | PROPERTY: PUBLIC ONLY
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets an attribute on the <see cref="GroupQueryElement"/> for determining whether groups loaded should be 
    ///   limited to those marked as "public".
    /// </summary>
    /// <remarks>
    ///   For privacy reasons, it is recommended that only public groups be archived (the default). Archiving closed or secret
    ///   groups may introduce privacy concerns depending on how the information is shared. It should be assumed that members of
    ///   a closed or secret group have an expectation of privacy, even if all users are granted access upon request.
    /// </remarks>
    [ConfigurationProperty("publicOnly", DefaultValue="true", IsRequired = false)]
    public bool PublicOnly {
      get {
        return Boolean.Parse((string)this["publicOnly"]);
      }
      set {
        this["publicOnly"] = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: FILTER
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets a filter to use when querying groups. 
    /// </summary>
    /// <remarks>
    ///   By default, the filter is set to "Skepti-Forum", meaning that only groups that contain the term "Skepti-Forum" in 
    ///   their name will be included in the query results. The filter looks for a literal match: a query for "Skepti Forum" for
    ///   instance would match "Skepti Forum", but not "Skepti-Forum", "Skeptics Forum", of "a Forum for Skeptics". 
    /// </remarks>
    [ConfigurationProperty("filter", DefaultValue="Skepti-Forum", IsRequired = false)]
    public string Filter {
      get {
        return (string)this["filter"];
      }
      set {
        this["filter"] = value;
      }
    }

  } //Class
} //Namespace
