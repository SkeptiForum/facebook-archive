(function () {
  'use strict';

/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Facebook Archive
\=============================================================================================================================*/
  angular
    .module('app')
    .controller('ThreadDetailController', ThreadDetailController);

  ThreadDetailController.$inject = ['$location', '$http'];

/*==============================================================================================================================
| CONTROLLER: THREAD DETAIL CONTROLLER
\-----------------------------------------------------------------------------------------------------------------------------*/
/** @ngdoc controller
  * @name app:ThreadDetailController
  * @description
  * Provides data access for thread details (via archived JSON content) and additionally offers functions for filtering comments
  * based on author and number of likes. 
  */
  function ThreadDetailController($location, $http) {
    /* jshint validthis:true */

  /*============================================================================================================================
  | ESTABLISH VARIABLES
  \---------------------------------------------------------------------------------------------------------------------------*/
    var vm = this;
    vm.post = null;
    vm.search = {
      from: {
        id: ''
      }
    };
    vm.likedFilter = likedFilter;
    vm.toggleAuthor = toggleAuthor;

    activate();

  /*============================================================================================================================
  | METHOD: ACTIVATE
  \---------------------------------------------------------------------------------------------------------------------------*/
  /** @ngdoc method
    * @name  app:ThreadDetailController#activate
    * @kind  function
    * @description Prepares the thread detail controller with necessary dependencies. This includes primarily the archived JSON
    * file, which is loaded based on routing data in the URL.
    */
    function activate() {

      var path = $location.path();
      var pathArray   = path.split('/');
      var groupId     = pathArray[2];
      var postId      = pathArray[4];

      $http.get('/Archives/' + groupId + '/' + groupId + '_' + postId + '.json').
        success(function(data, status, headers, config) {      
          vm.post = data;
        }
      );

    }

  /*============================================================================================================================
  | METHOD: LIKED FILTER
  \---------------------------------------------------------------------------------------------------------------------------*/
  /** @ngdoc method
    * @name  app:ThreadDetailController#likedFilter
    * @kind  function
    * @description Provides a helper function that the comment repeater can use to filter comments based on a minimum like 
    * count (assuming the corresponding checkbox is selected).
    * 
    * @param {object=} comment The instant of the comment being evaluated by the repeater.
    */
    var likedFilter = function (comment) {
      return !vm.likedOnly || (comment.like_count && comment.like_count > 1);
    };

  /*============================================================================================================================
  | METHOD: TOGGLE AUTHOR
  \---------------------------------------------------------------------------------------------------------------------------*/
  /** @ngdoc method
    * @name  app:ThreadDetailController#toggleAuthor
    * @kind  function
    * @description Provides a helper function that the comment repeater can use to filter comments based on whether or not they 
    * were written by the author of the original post (assuming the corresponding checkbox is selected).
    * 
    * @param {object=} comment The instant of the comment being evaluated by the repeater.
    */
    var toggleAuthor = function () {
      vm.search.from.id = vm.authorOnly ? vm.post.from.id : '';
    }

  }
})();
