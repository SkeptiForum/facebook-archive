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
  public static class ArchiveManager {

    /*==========================================================================================================================
    | PRIVATE VARIABLES
    \-------------------------------------------------------------------------------------------------------------------------*/
    private static ArchiveSection _configuration = null;
    private static FacebookGroupCollection _groups = null;
    private static ProviderCollection _dataProviders = new ProviderCollection();
    private static ProviderCollection _storageProviders = new ProviderCollection();
    private static ArchiveDataProviderBase _dataProvider = null;
    private static ArchiveStorageProviderBase _storageProvider = null;

    /*==========================================================================================================================
    | PROPERTY: ARCHIVE SECTION
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
    /// </summary>
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
    ///   
    /// </summary>
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
    ///   
    /// </summary>
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
    ///   
    /// </summary>
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
    ///   
    /// </summary>
    public static ArchiveStorageProviderBase StorageProvider {
      get {
        if (_storageProvider == null) {
          if (StorageProviders.Count == 0 || Configuration.DefaultStorageProvider == "FacebookStorageProvider") {
            _storageProvider = new FacebookStorageProvider();
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
    | PROPERTY: DATA PROVIDERS
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   
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
