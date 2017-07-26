(function () {
    'use strict';

    var app = angular.module('app', [
        // Angular modules
        'ngAnimate',
        'ngMessages',
        'ngCookies',

        // Custom modules

        // 3rd Party Modules
        'ui.bootstrap',
        'ui.router',
        'ui.mask'

    ]);

    app.config(function ($stateProvider, $urlRouterProvider, $httpProvider) {

       
        //
        // For any unmatched url, redirect to /state1
        $urlRouterProvider.otherwise("/logon");
        //
        // Now set up the states
        $stateProvider
            .state('logon', {
                url: "/logon",
                templateUrl: "app/logon/logon.html",
                controller: 'logonController',
                controllerAs: 'vmLogon'
            })
            .state('request', {
              url: "/request",
              templateUrl: "app/request/request.html",
              controller: 'requestController',
              controllerAs: 'vmRequest',
              params: { user: {value: null} }
            });
    });

    app.run(function($http) {
        $http.defaults.headers.common['Auth-Token'] = 'U1BBUktUT0tFTg==';
    });

    // should be on v=2
}());