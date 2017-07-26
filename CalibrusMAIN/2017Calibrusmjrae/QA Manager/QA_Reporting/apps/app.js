
(function () {
    "use strict";


    var app = angular.module('qaManager', ['ngRoute']);


    // var routes = [
    //     {
    //         url: "/a",
    //         settings: { templateUrl: "src/views/home.html" }
    //     }, {
    //         url: "/secret",
    //         settings: { templateUrl: "templates/secret.html" }
    //     }, {
    //         url: "/login",
    //         settings: { templateUrl: "templates/login.html" }
    //     }
    // ];

    app.config(function ($routeProvider, $locationProvider) {
        $routeProvider
            .when('/', {
                templateUrl: 'src/views/home.html',
                controller: 'mController'
            })
            .when('/second', {
                templateUrl: 'other.html',
                controller: 'mainController'
            })
            .otherwise({
                templateUrl: 'other.html'
            });

        // not consistent with results   /#/second  then /second  then refresh and not found 
        //$locationProvider.html5Mode({
        //  enabled: true,
        //  requireBase: false
        //});

        $locationProvider.hashPrefix(''); // get rid of !    was getting #! 

    });



    // var registerRoutes = function ($routeProvider) {
    //     routes.forEach(function (route) {
    //         $routeProvider.when(route.url, route.settings);
    //     });
    //     $routeProvider.otherwise({ redirectTo: routes[0].url });
    // };

    // app.config(registerRoutes);


    app.controller('mController', ['$scope', '$log', function ($scope, $log) {

    }]);

    //app.controller('mainController', ['$scope', '$routeParams', function ($scope, $routeParams) {
    //    console.log('in controller');
    //}]);


}());