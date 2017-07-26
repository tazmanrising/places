(function () {
    'use strict';

    var injectParams = ['$http', '$q'];


    var directiveService = function ($http, $q) {
        var baseUrl = "http://localhost:3500/api/";
        //var baseUrl = "http://10.100.40.206:3500/api/"; // $env.apiUrl + $env.apiBase;

        var factory = {};

        factory.getDirectives = function (req) {
            var url = "";
            //TODO  :   need to change to be dynamic 
            url = baseUrl + "liberty/directives";
            
            return $http.get(url).then(function (result) {
                //console.log('service',result.data);
                return result.data;

            }, function(err){
                console.log('err get directives', err);
            });

        }


        factory.getDirectiveAssoc = function(questionId){
            var url = "";
             url = baseUrl + "liberty/directiveassoc/" + questionId;
            
            return $http.get(url).then(function (result) {
                //console.log('service',result.data);
                return result.data;

            }, function(err){
                console.log('err get directives', err);
            });

        }


        return factory;
    };


    directiveService.$inject = injectParams;

    angular
        .module('calibrus')
        .factory('directiveService', directiveService);

}());