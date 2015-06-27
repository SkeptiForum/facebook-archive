(function () {
  'use strict';

  angular
    .module('app')
    .controller('ThreadDetailController', ThreadDetailController);

  ThreadDetailController.$inject = ['$location', '$http'];

  function ThreadDetailController($location, $http) {
    /* jshint validthis:true */
    var vm = this;

    vm.post = null;

    vm.search = {
      from: {
        id: ''
      }
    };

    vm.likedFilter = function (comment) {
      return !vm.likedOnly || (comment.like_count && comment.like_count > 1);
    };

    vm.toggleAuthor = function () {
      vm.search.from.id = vm.authorOnly ? vm.post.from.id : '';
    }

    activate();

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

  }
})();
