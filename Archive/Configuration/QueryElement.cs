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
  public class QueryElement : ConfigurationElement {

    /*==========================================================================================================================
    | PROPERTY: LIMIT
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
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
    ///   
    /// </summary>
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
