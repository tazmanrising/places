(function () {
    "use strict";


    console.log('test');


    // var env = {};

    // if (window) {
    //     Object.assign(env, window.__env);
    // }

    var app = angular.module('calibrus',
        [
            'ui.router',
            'toastr',
            'mgcrea.ngStrap'
        ]);


    // app.constant("rootUrl", "http://test/");  // not used
    // app.constant("$env", env);  // env.js  use in service/factories 


    app.config([
        '$stateProvider',
        '$urlRouterProvider',
        '$locationProvider',
        function ($stateProvider, $urlRouterProvider, $locationProvider) {

            var viewBase = '/views/';

            $urlRouterProvider.otherwise("/");

            $locationProvider.hashPrefix(''); // get rid of ! was getting #! 



            $stateProvider
                .state('algolia',
                {
                    url: "/algolia",
                    templateUrl: viewBase + "algolia.html",
                    controller: "algoliaController",
                    controllerAs: "vm"
                })
                .state('scriptquestions',
                {
                    url: "/scriptquestions",
                    templateUrl: viewBase + "scriptQuestions.html",
                    controller: "scriptQuestionController",
                    controllerAs: "vm"
                })
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