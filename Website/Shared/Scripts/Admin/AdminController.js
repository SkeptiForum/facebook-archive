(function () {
  'use strict';

/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Facebook Archive
\=============================================================================================================================*/
  angular
    .module('admin')
    .controller('AdminController', AdminController);

  AdminController.$inject = ['$location', '$http', '$facebook', '$cookies', '$q'];

/*==============================================================================================================================
| CONTROLLER: DOWNLOADCONTROLLER
\-----------------------------------------------------------------------------------------------------------------------------*/
/** @ngdoc controller
  * @name app:AdminController
  * @description
  * Provides a list of groups available for management on this site, including metadata for when they were last downloaded. 
  * Deduplicates this list against data the current user is authorized for, and provides options for downloading posts for 
  * any groups the user has permissions for.
  */
  function AdminController($location, $http, $facebook, $cookies, $q) {
    /* jshint validthis:true */

  /*============================================================================================================================
  | ESTABLISH VARIABLES
  \---------------------------------------------------------------------------------------------------------------------------*/
    var vm = this;
    vm.groups = null;
    vm.isAuthenticated = false;
    vm.login = function () {
      $facebook.login().then(function (response) {
        vm.isAuthenticated = true;
        processLogin(response);
      });
    }
    vm.archiveGroup = archiveGroup;
    vm.indexGroup = indexGroup;
    vm.logout = function () {
      $cookies.remove('fbat');
      $facebook.logout().then(function (response) {
        vm.isAuthenticated = false;
      });
    };
    vm.getState = function(group, method) {
      return {
        processing: group[method + "Processing"], 
        updated: group[method + "Updating"], 
        error: group[method + "Error"]
      }
    }

    activate();

  /*============================================================================================================================
  | METHOD: ACTIVATE
  \---------------------------------------------------------------------------------------------------------------------------*/
  /** @ngdoc method
    * @name  app:AdminController#activate
    * @kind  function
    * @description Prepares the thread detail controller with necessary dependencies. This includes primarily the archived JSON
    * file, which is loaded based on routing data in the URL.
    */
    function activate() {

      //First, check that the user is authenticated against Facebook
      $facebook.getLoginStatus().
        then(processLogin);

      //Meanwhile, download the groups that are associated with the site
      $http.get('/API/Groups/').
        success(function(data, status, headers, config) {      
          vm.siteGroups = data;
          collateData();
        }
      );

    }

  /*============================================================================================================================
  | METHOD: PROCESS LOGIN
  \---------------------------------------------------------------------------------------------------------------------------*/
  /** @ngdoc method
    * @name  app:AdminController#processLogin
    * @kind  function
    * @description Responds to a user's successful authentication by loading subsequent data that the user depends on, such as 
    * user groups.
    */
    function processLogin(response) {

      var authorizationPromise = $q.defer();

      if (response.status != "connected") {
        return authorizationPromise.reject();
      }

      var accessToken = response.authResponse.accessToken;
      var fbat = $cookies.get("fbat");

      if (!fbat) {
        $http.post(
          '/Api/Authorize',
          '"' + accessToken + '"'
        ).
        success(function (data, statusText) {
          authorizationPromise.resolve();
          vm.isAuthenticated = true;
        });
      }
      else {
        authorizationPromise.resolve();
        vm.isAuthenticated = true;
      }

      authorizationPromise.promise.then(function(response) {
        getUserGroups();
      });

      return authorizationPromise.promise;

    }

  /*============================================================================================================================
  | METHOD: GET USER GROUPS
  \---------------------------------------------------------------------------------------------------------------------------*/
  /** @ngdoc method
    * @name  app:AdminController#getUserGroups
    * @kind  function
    * @description Requests the user's groups from the Facebook Graph API.
    */
    function getUserGroups() {
      return $facebook.api("/me/groups?limit=250&fields=id").
        then(function(response) {
          vm.userGroups = response.data;
          collateData();
        });
    }

  /*============================================================================================================================
  | METHOD: COLLATE DATA
  \---------------------------------------------------------------------------------------------------------------------------*/
  /** @ngdoc method
    * @name  app:AdminController#collateData
    * @kind  function
    * @description Collates data from the Facebook Graph API's /groups edge and the local Web API's /groups endpoint into a 
    * single list that includes both data from the server as well as the user's own permissions. Additionally, set a new 'isOld'
    * property determining whether or not the group has been archived within the last two weeks.
    */
    function collateData() {
      if (!vm.siteGroups || !vm.userGroups) return;
      vm.siteGroups.forEach(function (siteGroup) {
        vm.userGroups.forEach(function (userGroup) {
          if (userGroup.id == siteGroup.Id) {
            siteGroup.isMemberOf = true;
          }
        });
        siteGroup.isArchiveStale = moment(siteGroup.LastArchived).diff(Date.now(), 'days') < -14;
        siteGroup.isIndexStale = moment(siteGroup.LastIndexed).diff(Date.now(), 'days') < -14;
      });
    }

  /*============================================================================================================================
  | METHOD: ARCHIVE GROUP
  \---------------------------------------------------------------------------------------------------------------------------*/
  /** @ngdoc method
    * @name  app:AdminController#archiveGroup
    * @kind  function
    * @description Requests that the server archive newer posts and comments from Facebook.
    */
    function archiveGroup(group) {
      group.ArchiveProcessing = true;
      $http.post('/Api/Groups/' + group.Id + '/Archive').
      success(function (data, statusText) {
        delete group.ArchiveProcessing;
        group.LastArchived = data.LastArchived;
        group.PostCount = data.PostCount;
        group.ArchiveUpdated = true;
        group.isIndexOld = false;
      }).
      error(function (data, statusText) {
        group.ArchiveError = true;
      });
    }

    /*============================================================================================================================
    | METHOD: INDEX GROUP
    \---------------------------------------------------------------------------------------------------------------------------*/
    /** @ngdoc method
      * @name  app:AdminController#indexGroup
      * @kind  function
      * @description Requests that the server index newer posts and comments from Facebook.
      */
    function indexGroup(group) {
      group.IndexProcessing = true;
      $http.post('/Api/Groups/' + group.Id + '/Index').
      success(function (data, statusText) {
        delete group.IndexProcessing;
        group.LastIndexed = data.LastIndexed;
        group.IndexUpdated = true;
        group.isIndexOld = false;
      }).
      error(function (data, statusText) {
        group.IndexError = true;
      });
    }

  }
})();
