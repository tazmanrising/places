(function () {
    'use strict';
    var injectParams = ['$http', '$q'];

    var scriptQuestionService = function($http, $q) {
        var baseUrl = "http://localhost:3500/api/";
        //var baseUrl = "http://10.100.40.206:3500/api/"; //$env.apiUrl + $env.apiBase;
        //console.log(url);

        var factory = {};


        factory.getStates = function() {
            var url = "";
            url = baseUrl + 'liberty/states';

            return $http.get(url).then(function (result){
                return result.data;
            })
        }

        factory.getSalesChannel = function(){
            var url;
            url = baseUrl + "liberty/saleschannel";
            return $http.get(url).then(function(result){
                return result.data;
            });
        };


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

         factory.createScriptQuestion = function(req) {
            console.log('createQuestion req', req);
            var url = ""; 
            url = baseUrl + "liberty/scriptquestion";
            
            return $http.post(url, req)
                .then(function(result){
                    console.log('add scriptquestion result', result);
                    return result.data;
                })

        }

        factory.getAllScriptQuestions = function(statecode,saleschannel) {
    
            var url = ""; 
            url = baseUrl + "liberty/scriptquestions/"+ statecode + "/" + saleschannel;
            
            return $http.get(url).then(function (result) {
                //console.log('service',result.data);
                return result.data;
                
            });

        }

         factory.updateScriptQuestion = function(req) {
            console.log('updateQuestion req', req);
            var url = "";
            url = baseUrl + "liberty/scriptquestion";

            return $http.put(url, req)
                .then(function(result){
                    console.log('update script question result', result);
                    return result.data;
                })

        }
        



        return factory;

    };

 



    scriptQuestionService.$inject = injectParams;

    angular
        .module('calibrus')
        .factory('scriptQuestionService', scriptQuestionService);
    //.factory('airportCodeService', airportCodeService)


}());