(function () {
    'use strict';

    var injectParams = ['$http', '$q', '$env'];


    var questionService = function($http, $q, $env) {

        var url = $env.apiUrl + $env.apiBase + 'GetAllQuestions';
        //console.log(url);

        var factory = {};

        factory.getAllQuestions = function() {
            return $http.get(url).then(function(result) {
                //console.log('service',result.data);
                return result.data;
                
            });

        }


        return factory;

    };

 



    questionService.$inject = injectParams;

    angular
        .module('calibrus')
        .factory('questionService', questionService);
    //.factory('airportCodeService', airportCodeService)


}());