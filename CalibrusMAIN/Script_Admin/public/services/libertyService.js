(function () {
    'use strict';

    var libertyService = function ($http, $q) {
          var baseUrl = "http://localhost:3500/api/";
        //var baseUrl = "http://10.100.40.206:3500/api/"; //$env.apiUrl + $env.apiBase;
        var factory = {};

        factory.getLibertyQuestions = function () {
            var url = "";
            url = baseUrl + "liberty/scriptquestions";

            return $http.get(url).then(function (result) {
                //console.log('service',result.data);
                return result.data;

            });

        }

        factory.getLibertyMarketState = function(){
            var url = "";
            url = baseUrl + "liberty/MarketState";
             return $http.get(url).then(function(result){
                return result.data;
            }, function(err){
                console.log('err with get Market State', err);
            });

        }
        

        factory.validateAgent = function(id){

            var url = "";
            console.log('client id', id);
            url = baseUrl + "liberty/validateAgent/" + id;
            return $http.get(url).then(function(result){
                return result.data;
            }, function(err){
                console.log('err getting agent validation', err);
            });
        }


        factory.getDirectives = function(id) {
            var url = "";
            url = baseUrl + "liberty/questiondirectives/" + id;

            return $http.get(url).then(function(result){
                return result.data;
            }, function(err){
                console.log('err getLibertyquestiondirectives',err);
            });

        }

        return factory;

    }


    angular
        .module('calibrus')
        .factory('libertyService', libertyService);


}());