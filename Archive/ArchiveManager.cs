/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
\=============================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Configuration.Provider;
using System.Web.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SkeptiForum.Archive.Providers;
using SkeptiForum.Archive.Configuration;

namespace SkeptiForum.Archive {

  /*============================================================================================================================
  | CLASS: ARCHIVE MANAGER
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides static access to the archive site's configuration, including the web.config values, list of groups, as well as
  ///   the data and storage providers. 
  /// </summary>
  /// <remarks>
  ///   This class acts as the primary entry point for access to the data and storage provider, via the <see 
  ///   cref="DataProvider"/> and <see cref="StorageProvider"/> properties.
  /// </remarks>
  public static class ArchiveManager {

    /*==========================================================================================================================
    | PRIVATE VARIABLES
    \-------------------------------------------------------------------------------------------------------------------------*/
    private static ArchiveSection               _configuration = null;
    private static FacebookGroupCollection      _groups = null;
    private static ProviderCollection           _dataProviders = new ProviderCollection();
    private static ProviderCollection           _storageProviders = new ProviderCollection();
    private static ArchiveDataProviderBase      _dataProvider = null;
    private static ArchiveStorageProviderBase   _storageProvider = null;

    /*==========================================================================================================================
    | PROPERTY: ARCHIVE SECTION
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides a reference to the current configuration for the application from the web.config file. 
    /// </summary>
    /// <remarks>
    ///   The web.config stores information such as query limits, default fields, API keys, and storage location. 
    /// </remarks>
    public static ArchiveSection Configuration {
      get {
        if (_configuration == null) {
          _configuration = (ArchiveSection)ConfigurationManager.GetSection("archive");
          if (_configuration == null) {
            throw new ConfigurationErrorsException("The Archive configuration section (<archive />) is not set correctly.");
          }
        }
        return _configuration;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: GROUPS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides access to the list of groups configured for this site, along with any state information associated with them.
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     State information may include, for instance, the last date that the groups were archived, and may be written to by a
    ///     variety of tools. 
    ///   </para>
    ///   <para>
    ///     The groups are configured locally and not pulled directly from Facebook. This ensures that all users have an ongoing
    ///     reference to the groups regardless of their app access or rights. That said, tools that rely on the Facebook API to 
    ///     interact with groups should verify the user's access by comparing the list of configured groups against their 
    ///     individual group membership.
    ///   </para>
    /// </remarks>
    public static FacebookGroupCollection Groups {
      get {
        if (_groups == null) {
          _groups = FacebookGroupCollection.CreateFromStorageProvider().Result;
        }
        return _groups;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: DATA PROVIDER
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Returns a reference to the default data provider. 
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This property should be the primary entry point for any requests to providers, thus ensuring that only the currently
    ///     configured provider is used. 
    ///   </para>
    ///   <para>
    ///     If no data providers are configured via the <see cref="DataProviders"/> collection, and the <see 
    ///     cref="Configuration.ArchiveSection.DefaultDataProvider"/> does not contain a default provider, then the archive 
    ///     manager will instantiate a new <see cref="FacebookDataProvider"/> instance and return that.
    ///   </para>
    /// </remarks>
    public static ArchiveDataProviderBase DataProvider {
      get {
        if (_dataProvider == null) {
          if (DataProviders.Count == 0 || Configuration.DefaultDataProvider == "FacebookDataProvider") {
            _dataProvider = new FacebookDataProvider();
          }
          else if (String.IsNullOrWhiteSpace(Configuration.DefaultDataProvider)) {
            _dataProvider = (ArchiveDataProviderBase)DataProviders[Configuration.DefaultDataProvider];
          }
          else {
            throw new Exception("The defaultDataProvider value is not available from the Archive configuration section (<archive />)");
          }
        }
        return _dataProvider;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: DATA PROVIDERS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Provides a list of all configured data providers for the application. 
    /// </summary>
    /// <remarks>
    ///   Typically, access will be provided via the <see cref="DataProvider"/> property instance. The <see 
    ///   cref="DataProviders"/> property provides a convenient way of iterating across all providers for instances where more 
    ///   than one provider may be used.
    /// </remarks>
    public static ProviderCollection DataProviders {
      get {
        if (_dataProviders.Count == 0) {
          ProvidersHelper.InstantiateProviders(Configuration.DataProviders, _dataProviders, typeof(ArchiveDataProviderBase));
          _dataProviders.SetReadOnly();
        }
        return _dataProviders;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: STORAGE PROVIDER
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    /// <summary>
    ///   Returns a reference to the default storage provider. 
    /// </summary>
    /// <remarks>
    ///   <para>
    ///     This property should be the primary entry point for any requests to providers, thus ensuring that only the currently
    ///     configured provider is used. 
    ///   </para>
    ///   <para>
    ///     If no storage providers are configured via the <see cref="StorageProviders"/> collection, and the <see 
    ///     cref="Configuration.ArchiveSection.DefaultStorageProvider"/> does not contain a default provider, then the archive 
    ///     manager will instantiate a new <see cref="FileSystemStorageProvider"/> instance and return that.
    ///   </para>
    /// </remarks>
    /// </summary>
    public static ArchiveStorageProviderBase StorageProvider {
      get {
        if (_storageProvider == null) {
          if (StorageProviders.Count == 0 || Configuration.DefaultStorageProvider == "FileSystemStorageProvider") {
            _storageProvider = new FileSystemStorageProvider();
          }
          else if (String.IsNullOrWhiteSpace(Configuration.DefaultDataProvider)) {
            _storageProvider = (ArchiveStorageProviderBase)StorageProviders[Configuration.DefaultStorageProvider];
          }
          else {
            throw new Exception("The defaultStorageProvider value is not available from the Archive configuration section (<archive />)");
          }
        }
        return _storageProvider;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: STORAGE PROVIDERS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    /// <summary>
    ///   Provides a list of all configured storage providers for the application. 
    /// </summary>
    /// <remarks>
    ///   Typically, access will be provided via the <see cref="StorageProvider"/> property instance. The <see 
    ///   cref="StorageProviders"/> property provides a convenient way of iterating across all providers for instances where more 
    ///   than one provider may be used.
    /// </remarks>
    /// </summary>
    public static ProviderCollection StorageProviders {
      get {
        if (_storageProviders.Count == 0) {
          ProvidersHelper.InstantiateProviders(Configuration.StorageProviders, _dataProviders, typeof(ArchiveStorageProviderBase));
          _storageProviders.SetReadOnly();
        }
        return _storageProviders;
      }
    }

  } //Class
} //Namespace
