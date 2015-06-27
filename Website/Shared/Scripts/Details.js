(function () {
  'use strict';

  angular
    .module('app')
    .controller('Details', Details);

  Details.$inject = ['$location'];

  function Details($location) {
    /* jshint validthis:true */
    var vm = this;
    vm.title = 'Details';

    activate();

    function activate() {
    }

  }
})();
