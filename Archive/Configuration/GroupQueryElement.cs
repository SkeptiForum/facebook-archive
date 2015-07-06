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
  public class GroupQueryElement : QueryElement {

    /*==========================================================================================================================
    | PROPERTY: PUBLIC ONLY
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
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
    ///   
    /// </summary>
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
