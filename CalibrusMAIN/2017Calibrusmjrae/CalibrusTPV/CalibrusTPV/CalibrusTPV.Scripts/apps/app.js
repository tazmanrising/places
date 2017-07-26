(function() {
    "use strict";

    var env = {};

    if (window) {
        Object.assign(env, window.__env);
    }
    
    var app = angular.module('calibrus',
    [
        'ui.router', 'ui.grid'
    ]);


    app.constant("rootUrl", "http://test/");  // not used
    app.constant("$env", env);  // env.js  use in service/factories 


    app.config([
        '$stateProvider',
        '$urlRouterProvider',
        '$locationProvider',
        function ($stateProvider, $urlRouterProvider, $locationProvider) {

            var viewBase = '/apps/src/views/';

            $urlRouterProvider.otherwise("/");

            $locationProvider.hashPrefix(''); // get rid of ! was getting #! 

            $stateProvider
                .state('questions',
                {
                    url: "/questions",
                    templateUrl: viewBase + "questions.html",
                    controller: "questionController",
                    controllerAs: "vm"
                    //views: {
                    //
                    //}
                })
                .state("editquestion",
                {
                    url: "/editquestion",
                    templateUrl: viewBase + "questionEdit.html",
                    controller: "QuestionEditController",
                    controllerAs: "vm"
                });


        }]
    );


}());