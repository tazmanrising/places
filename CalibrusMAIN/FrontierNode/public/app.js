(function() {
    "use strict";

    //var env = {};

    //if (window) {
    //    Object.assign(env, window.__env);
    //}
    
    var app = angular.module('calibrus',
    [
        'ui.router', 'toastr','mgcrea.ngStrap'
    ]);


    //app.constant("rootUrl", "http://test/");  // not used
    //app.constant("$env", env);  // env.js  use in service/factories 


    app.config([
        '$stateProvider',
        '$urlRouterProvider',
        '$locationProvider',
        
        function ($stateProvider, $urlRouterProvider, $locationProvider) {

            var viewBase = '/views/';

            //$urlRouterProvider.otherwise("/");


            $urlRouterProvider.otherwise("/questions/:id/:agentid/:agent");
            
            // $urlRouterProvider.otherwise(function($injector, $location) {
            //     console.log("Could not find " + $location);
            //     $location.path('/questions');
            // });
            
            $locationProvider.hashPrefix(''); // get rid of ! was getting #! 

            $stateProvider
                .state('scriptquestions',
                {
                    url: "/scriptquestions",
                    templateUrl: viewBase + "scriptQuestions.html",
                    controller: "scriptQuestionController",
                    controllerAs: "vm"
                })
                .state('questions',
                {
                    url: "/questions/:id/:agentid/:agent/:language",
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