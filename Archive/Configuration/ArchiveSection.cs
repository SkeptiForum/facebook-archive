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
  | CLASS: ARCHIVE SECTION
  \---------------------------------------------------------------------------------------------------------------------------*/
  public class ArchiveSection : ConfigurationSection {

    /*==========================================================================================================================
    | PROPERTY: API
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    [ConfigurationProperty("api", IsRequired = true)]
    public ApiElement Api {
      get {
        return (ApiElement)this["api"];
      }
      set {
        this["api"] = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: QUERIES
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    [ConfigurationProperty("queries", IsRequired = false)]
    public QueriesElement Queries {
      get {
        return (QueriesElement)this["queries"];
      }
      set {
        base["queries"] = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: STORAGE DIRECTORY
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    [ConfigurationProperty("storageDirectory", DefaultValue = "/Archive/", IsRequired = false)]
    public string StorageDirectory {
      get {
        return (string)this["storageDirectory"];
      }
      set {
        this["storageDirectory"] = value;
      }
    }

    /*============================================================================================================================
    | PROPERTY: DEFAULT DATA PROVIDER
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    [ConfigurationProperty("defaultDataProvider", DefaultValue = "FacebookProvider", IsRequired = false)]
    public string DefaultDataProvider {
      get {
        return (string)this["defaultDataProvider"];
      }
      set {
        this["defaultDataProvider"] = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: DATA PROVIDERS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    [ConfigurationProperty("dataProviders", IsRequired = false)]
    public ProviderSettingsCollection DataProviders {
      get {
        return (ProviderSettingsCollection)base["dataProviders"];
      }
    }

    /*============================================================================================================================
    | PROPERTY: DEFAULT STORAGE PROVIDER
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    [ConfigurationProperty("defaultStorageProvider", DefaultValue = "FacebookProvider", IsRequired = false)]
    public string DefaultStorageProvider {
      get {
        return (string)this["defaultStorageProvider"];
      }
      set {
        this["defaultStorageProvider"] = value;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: STORAGE PROVIDERS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
    [ConfigurationProperty("storageProviders", IsRequired = false)]
    public ProviderSettingsCollection StorageProviders {
      get {
        return (ProviderSettingsCollection)base["storageProviders"];
      }
    }

  } //Class
} //Namespace
