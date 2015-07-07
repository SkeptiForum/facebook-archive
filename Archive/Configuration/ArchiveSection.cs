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
  /// <summary>
  ///   Establishes a root section in the application's web.config file for configuring the archive. 
  /// </summary>
  public class ArchiveSection : ConfigurationSection {

    /*==========================================================================================================================
    | PROPERTY: API
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets or sets access to an "api" element, which is responsible for configuring the Facebook API client. 
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
    ///   Gets or sets access to a "queries" element, which includes default parameters for different types of queries that the 
    ///   data provider will need for perform. For instance, it can be used to set the limit or default fields for querying 
    ///   groups, posts, or comments.
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
    ///   Gets or sets an attribute on the root section for determining the location that archived data should be stored. 
    /// </summary>
    /// <remarks>
    ///   The usage of this attribute may vary depending on the storage medium. For the default 
    ///   <see cref="SkeptiForum.Archive.Providers.FacebookStorageProvider"/>, this repesents the relative path to the folder
    ///   where the files will be stored. By default, the value is set to "/Archive/".
    /// </remarks>
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
    ///   Gets or sets an attribute on the root element representing which configured data provider to use. 
    /// </summary>
    /// <remarks>
    ///   By default, assuming no data providers are configured, the value will be set to 
    ///   <see cref="SkeptiForum.Archive.Providers.FacebookDataProvider"/>, which implements the Facebook SDK for .NET from 
    ///   Outercurve Foundation.
    /// </remarks>
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
    ///   Gets or sets a child "dataProviders" element which allows configuration of a collection of data providers.
    /// </summary>
    /// <remarks>
    ///   Data providers are responsible for providing access to the social backend, by default Facebook. Data providers must 
    ///   inherit from <see cref="SkeptiForum.Archive.Providers.ArchiveDataProviderBase"/>.
    /// </remarks>
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
    ///   Gets or sets an attribute on the root element representing which configured storage provider to use. 
    /// </summary>
    /// <remarks>
    ///   By default, assuming no storage providers are configured, the value will be set to 
    ///   <see cref="SkeptiForum.Archive.Providers.FacebookStorageProvider"/>, which storage group configuration and post data
    ///   to the harddrive.
    /// </remarks>
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
    ///   Gets or sets a child "storageProviders" element which allows configuration of a collection of storage providers.
    /// </summary>
    /// <remarks>
    ///   Storage providers are responsible for archiving data to a persistence medium. Data providers must 
    ///   inherit from <see cref="SkeptiForum.Archive.Providers.ArchiveDataProviderBase"/>.
    /// </remarks>
    [ConfigurationProperty("storageProviders", IsRequired = false)]
    public ProviderSettingsCollection StorageProviders {
      get {
        return (ProviderSettingsCollection)base["storageProviders"];
      }
    }

  } //Class
} //Namespace
