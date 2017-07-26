(function () {
    'use strict';

    var googleService = function ($http, $q) {
          var baseUrl = "http://localhost:3500/api/";
        //var baseUrl = "http://10.100.40.206:3500/api/"; //$env.apiUrl + $env.apiBase;
        var factory = {};

        factory.getCityStateByZipCode = function () {
            var url = "";
            url = baseUrl + "google/zipcode/" + id;
            return $http.get(url).then(function (result) {
                return result.data;
            }, function (err) {
                console.log('err with get Market State', err);
            });

        }

        return factory;


    }


    angular
        .module('calibrus')
        .factory('googleService', googleService);


}());


