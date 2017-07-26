(function () {
    'use strict';

    var injectParams = ['$http', '$q'];
   
   


    var questionService = function ($http, $q) {

        //var baseUrl = $env.apiUrl + $env.apiBase;
        
        var baseUrl = "http://10.100.40.206:3500/api/";
        //var baseUrl = "http://localhost:3500/api/";

        //console.log(url);

        var factory = {};

        factory.getCustomer = function (tn) {
            var url = "";
            url = baseUrl + "frontier/customer/"+ tn;

            return $http.get(url).then(function(result){
                console.log('result', result);
                return result.data;
            });
            
        };

        factory.updateFrontier911 = function (req) {
            console.log('updateQuestion req', req);
            var url = "";
            url = baseUrl + "frontier/customerUpdate";

            return $http.put(url, req)
                .then(function (result) {
                    console.log('update question result', result);
                    return result.data;
                })

        }



        factory.getAllQuestions = function () {

            var url = "";
            url = baseUrl + "liberty/questions";


            return $http.get(url).then(function (result) {
                //console.log('service',result.data);
                return result.data;

            });

        }

        factory.createQuestion = function (req) {
            console.log('createQuestion req', req);
            var url = "";
            url = baseUrl + "liberty/question";

            return $http.post(url, req)
                .then(function (result) {
                    console.log('add question result', result);
                    return result.data;
                })

        }

        factory.updateQuestion = function (req) {
            console.log('updateQuestion req', req);
            var url = "";
            url = baseUrl + "liberty/question";

            return $http.put(url, req)
                .then(function (result) {
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