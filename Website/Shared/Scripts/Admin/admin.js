(function () {
  'use strict';

/*==============================================================================================================================
| Author        Jeremy Caney (Jeremy@Caney.net)
| Client        Skepti-Forum Project
| Project       Facebook Archive
\=============================================================================================================================*/
  angular
    .module('admin', [
      // Angular modules 
      'ngCookies',

      // Custom modules 

      // 3rd Party Modules
      'angularMoment',
      'ngFacebook'

    ])

    .config(function ($locationProvider, $facebookProvider) {
      //$locationProvider.html5Mode(true);
      $facebookProvider.setAppId('1648003478762169');
      $facebookProvider.setPermissions("user_groups");
    })

    .run(function ($rootScope) {

    //Instantiate Facebook
      (function (d, s, id) {
        var js, fjs = d.getElementsByTagName(s)[0];
        if (d.getElementById(id)) return;
        js = d.createElement(s); js.id = id;
        js.src = "//connect.facebook.net/en_US/sdk.js";
        fjs.parentNode.insertBefore(js, fjs);
      }(document, 'script', 'facebook-jssdk'));

    });

})();