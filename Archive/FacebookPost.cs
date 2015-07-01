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

namespace SkeptiForum.Archive {

  /*============================================================================================================================
  | CLASS: FACEBOOK POST
  \---------------------------------------------------------------------------------------------------------------------------*/
  /// <summary>
  ///   Provides a convenience wrapper to a dynamic JSON object representing Facebook's post data. Includes a number of helper
  ///   properties and methods for accessing frequently requested fields.
  /// </summary>
  public class FacebookPost {

    /*==========================================================================================================================
    | PRIVATE VARIABLES
    \-------------------------------------------------------------------------------------------------------------------------*/
      dynamic _post = "";

    /*==========================================================================================================================
    | CONSTRUCTOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Initializes a new instance of the <see cref="FacebookPost"/> class based on the response from the <see 
    ///   cref="FacebookClient.Get(string)"/> operation (such as that utilized by the <see 
    ///   cref="FacebookGroupCollection.LoadGroups(bool?, string)"/> method). 
    /// </summary>
    /// <param name="group">
    ///   The dynamic object representing the JSON response from the Facebook Graph API.
    /// </param>
    public FacebookPost(dynamic post) {
      _post = post;
    }

    /*==========================================================================================================================
    | PROPERTY: JSON
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the access to the underlying JSON document that the current object instance is based upon.
    /// </summary>
    public dynamic Json {
      get {
        return _post;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: IDENTIFIER
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the unique numeric identifier associated with a Facebook post.
    /// </summary>
    public long Id {
      get {
        return _post.id;
      }
    }

    /*==========================================================================================================================
    | PROPERTY: TITLE
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets a friendly title associated with the post. Since Facebook posts don't actually have titles, this is constructed
    ///   by assembling the user's name, date created, and forum name.
    /// </summary>
    public string Title {
      get {
        return Group.Name + " thread by " + Author + " (" + CreateDate.ToString("yyyy-MM-dd") + ")";
      }
    }

    /*==========================================================================================================================
    | PROPERTY: AUTHOR
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the name of the author that posted the thread.
    /// </summary>
    public string Author {
      get {
        if (_post.from != null) {
          return _post.from.name;
        }
        else {
          return "[Deleted User]";
        }
      }
    }

    /*==========================================================================================================================
    | PROPERTY: GROUP
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the group that the thread was posted to.
    /// </summary>
    public FacebookGroup Group {
      get {
        if (_post.to != null && _post.to.data.Count > 0) {
          return new FacebookGroup(_post.to.data[0]);
        }
        else {
          return new FacebookGroup(0, "");
        }
      }
    }

    /*==========================================================================================================================
    | PROPERTY: CREATE DATE
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the date that the post was originally created.
    /// </summary>
    public DateTime CreateDate {
      get {
        return DateTime.Parse(_post.created_time.ToString());
      }
    }

    /*==========================================================================================================================
    | PROPERTY: COMMENT COUNT
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the number of comments associated with the post. 
    /// </summary>
    public int CommentCount {
      get {
        if (_post.comments == null || _post.comments.data == null) {
          return 0;
        }
        else {
          return _post.comments.data.Count;
        }
      }
    }
    
    /*==========================================================================================================================
    | PROPERTY: LIKE COUNT
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the number of likes associated with the post. 
    /// </summary>
    public int LikeCount {
      get {
        if (_post.likes == null || _post.likes.data == null) {
          return 0;
        }
        else {
          return _post.likes.data.Count;
        }
      }
    }

    /*==========================================================================================================================
    | PROPERTY: MESSAGE
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the message associated with the post.
    /// </summary>
    public string Message {
      get {
        if (_post.message == null) {
          return "";
        }
        else {
          return _post.message;
        }
      }
    }

    /*==========================================================================================================================
    | METHOD: TRUNCATE MESSAGE
    \-------------------------------------------------------------------------------------------------------------------------*/
    /// <summary>
    ///   Gets the message associated with the post, truncated to the nearest character.
    /// </summary>
    /// <param name="length">
    ///   The desired length of the string returned; the result will be no longer than this number, excluding ellipsis. 
    /// </param>
    public string TruncateMessage(int length = 150) {
      var message = Message.Replace("\n", " ");
      if (message.Length > length) {
        var truncateAt = message.LastIndexOf(' ', length);
        if (truncateAt < 0) truncateAt = length; 
        message = message.Substring(0, truncateAt) + "...";
      }
      return message;
    }

  } //Class
} //Namespace
