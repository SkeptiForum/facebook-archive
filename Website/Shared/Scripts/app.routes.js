(function () {
  'use strict';

/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Facebook Archive
\=============================================================================================================================*/
  angular
  .module('app')
  .config(['$routeProvider', configureRoutes]);

  configureRoutes.$inject = ['$routeProvider'];

/*==============================================================================================================================
| CONFIGURE ROUTES
\-----------------------------------------------------------------------------------------------------------------------------*/
/** @name app:configureRoutes
  * @description
  * Configures routes associated with the Angular application. These are intended for use in a single-page application (SPA), 
  * although the website currently operates as a multi-page application with pre-rendering performed on the server-side. 
  */
  function configureRoutes($routeProvider) {
    $routeProvider
      .when('/groups', {
        templateUrl: '/Groups/Index.html',
      })
      .when('/groups/:groupId', {
        templateUrl: '/Threads/Index.html',
        controller: 'ThreadIndexController',
        controllerAs: 'vm'
      })
      .when('/groups/:groupId/permalink/:postId', {
        templateUrl: '/Threads/Detail.html',
        controller: 'ThreadDetailController',
        controllerAs: 'vm'
      })
      .otherwise({
        redirectTo: '/groups'
      });
  }

})();