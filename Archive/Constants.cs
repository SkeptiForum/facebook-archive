/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Forum Archive
>===============================================================================================================================
| ### NOTE JJC270615: For the application to work, this file must contain the correct Facebook ClientId and ClientSecret. For 
| security reasons, these are not included with the public repository, and updates to this file are prevented via the 
| .gitignore. For the application to run correctly, please contact the administrator of the application for access to the 
| correct values.
\=============================================================================================================================*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Facebook;

namespace SkeptiForum.Archive {

  /*============================================================================================================================
  | CLASS: CONSTANTS
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Stores constants associated with the Facebook Client, namely the application identifier and secret.
  /// </summary>
  public static class Constants {

    /*==========================================================================================================================
    | PROPERTY: CLIENT ID
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   The unique identifier of the Facebook App associated with the project.
    /// </summary>
    public static long ClientId {
      get {
        throw new NotImplementedException("The application must be configured with a ClientId in the Constants.cs file.");
      }
    }

    /*==========================================================================================================================
    | PROPERTY: CLIENT SECRET
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   The application secret of the Facebook App associated with the project. 
    /// </summary>
    /// <remarks>
    ///   Important: This value should not be published externally, and must be kept secret.
    /// </remarks>
    public static string ClientSecret {
      get {
        throw new NotImplementedException("The application must be configured with a ClientSecret in the Constants.cs file.");
      }
    }

  } //Class
} //Namespace
