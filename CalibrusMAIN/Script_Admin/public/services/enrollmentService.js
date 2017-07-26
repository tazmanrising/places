(function () {
    'use strict';

    var enrollmentService = function ($http, $q) {
        var baseUrl = "http://localhost:3500/api/";
        //var baseUrl = "http://10.100.40.206:3500/api/"; //$env.apiUrl + $env.apiBase;
        var factory = {};

        factory.validateAgent = function (id) {

            var url = "";
            console.log('client id', id);
            url = baseUrl + "liberty/validateAgent/" + id;
            return $http.get(url).then(function (result) {
                return result.data;
            }, function (err) {
                console.log('err getting agent validation', err);
            });
        }
        return factory;

    }


    angular
        .module('calibrus')
        .factory('enrollmentService', enrollmentService);


}());

angular
    .module('calibrus')
    .factory("CustomerService", ['$filter', '$http', function ($filter, $http) {

        var baseUrl = "http://localhost:3500/api/";
        //var baseUrl = "http://10.100.40.206:3500/api//";  //$env.apiUrl + $env.apiBase;
        var service = {};


        var utilityType = [
            { "type": "Electric" },
            { "type": "Gas" },
            { "type": "Electric & Gas" },
        ];

        var utilities = [
            { "MarketUtilityId": 20, "MarketState": "NY", "UtilityName": "Consolidated Edison" },
            { "MarketUtilityId": 21, "MarketState": "NY", "UtilityName": "National Grid" },
            { "MarketUtilityId": 22, "MarketState": "CT", "UtilityName": "New York State Electric & Gas" },
            { "MarketUtilityId": 23, "MarketState": "CT", "UtilityName": "Rochester Gas & Electric" },
        ];

      

        var countrylist = [
            { "id": 1, "country": "USA" },
            { "id": 2, "country": "Canada" },
            { "id": 3, "country": "India" },
        ];


        var statelist = [
            { "Id": 1, "state": "Alaska", "countryId": 1 },
            { "Id": 2, "state": "California", "countryId": 1 },
            { "Id": 3, "state": "New York", "countryId": 1 },
            { "Id": 4, "state": "New Brunswick", "countryId": 2 },
            { "Id": 5, "state": "Manitoba", "countryId": 2 },
            { "Id": 6, "state": "Delhi", "countryId": 3 },
            { "Id": 7, "state": "Bombay", "countryId": 3 },
            { "Id": 8, "state": "Calcutta", "countryId": 3 }
        ];

        service.getCountry = function () {
            return countrylist;
        };

        service.getCountryState = function (countryId) {
            var states = ($filter('filter')(statelist, { countryId: countryId }));
            return states;
        };

        service.getUtilityTypes = function () {
            return utilityType;
        }


        service.getUtilities = function (util) {
            //console.log('util', util);
            //var utils = ($filter('filter')(utilities, { MarketState: util }));
            //return utils;

            if(util == "NY"){
                id = 8;
            }

            var url = "";
            console.log('state id', id);
            url = baseUrl + "liberty/marketutility/" + id;
            return $http.get(url).then(function (result) {
                console.log('result.data', result.data);
                return result.data;
            }, function (err) {
                console.log('err getting agent validation', err);
            });

        }

        return service;


    }]);
