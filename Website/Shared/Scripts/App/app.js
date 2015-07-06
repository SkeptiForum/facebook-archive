(function () {
  'use strict';

/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Facebook Archive
\=============================================================================================================================*/
  angular
    .module('app', [
      // Angular modules 

      // Custom modules 

      // 3rd Party Modules
      'angularMoment'

    ])

    .config(function ($locationProvider) {
      $locationProvider.html5Mode(true);
    });

})();