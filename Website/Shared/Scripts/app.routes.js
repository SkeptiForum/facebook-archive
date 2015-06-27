angular
  .module('app')
  .config(['$routeProvider', configureRoutes]);

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
