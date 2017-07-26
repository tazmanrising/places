(function () {
    'use strict';

    var injectParams = ['$http', '$q', '$env'];


    var questionService = function($http, $q, $env) {

        var baseUrl = $env.apiUrl + $env.apiBase;
        //console.log(url);

        var factory = {};

        factory.getAllQuestions = function() {

            var url = ""; 
            url = baseUrl + "liberty/questions";


            return $http.get(url).then(function (result) {
                //console.log('service',result.data);
                return result.data;
                
            });

        }

        factory.createQuestion = function(req) {
            console.log('createQuestion req', req);
            var url = "";
            url = baseUrl + "liberty/question";

            return $http.post(url, req)
                .then(function(result){
                    console.log('add question result', result);
                    return result.data;
                })

        }

        factory.updateQuestion = function(req) {
            console.log('updateQuestion req', req);
            var url = "";
            url = baseUrl + "liberty/question";

            return $http.put(url, req)
                .then(function(result){
                    console.log('update question result', result);
                    return result.data;
                })

        }
        


        return factory;

    };

 



    questionService.$inject = injectParams;

    angular
        .module('calibrus')
        .factory('questionService', questionService);
    //.factory('airportCodeService', airportCodeService)


}());