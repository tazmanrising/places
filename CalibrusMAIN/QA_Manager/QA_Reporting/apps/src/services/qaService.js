(function () {
    'use strict';

    var injectParams = ['$http', '$q'];


    // single call living in memory  - modify and use later
    var qaService = function ($http, $q) {

        //var serviceBase = '/svc/air/v1/getallairlines';
        var serviceBase = 'http://localhost:5762/GetCalls';
        var factory = {};
        
        
        // ====  singleton persistence  ===
        var qaPromise = $http.get(serviceBase);

        factory.getAllQAList = function () {
            //return $http.jsonp(serviceBase + 'GetCalls').then(function (results){
            //  console.log(results.data);
            //  return formatQA(results.data, code);
            //});

            //return airlinePromise.then(function (results) {
            //    return findAirport(results.data, code)
            //});
           
                return qaPromise.then(function (results) {
                    //console.log(results.data);
                    return results.data;
                    //return formatQA(results.data)
                });
                
            



        }

        function formatQA(qaList) {
            // ? filtering

            return qaList;

        }


        // function findAirport(airportList, code) {

        //     var airLen = airportList.length;
        //     var res = "";

        //     for (var i = 0; i < airLen; i++) {
        //         var item = airportList[i];
        //         if (item.carrierId == code) {
        //             res = item.airline;
        //             break;
        //         }

        //     }
        //     return res;
        // }

        return factory;
    };


    //airportCodeService.$inject = injectParams;
    qaService.$inject = injectParams;

    angular
        .module('qaManager')
        .factory('qaService', qaService)
    //.factory('airportCodeService', airportCodeService)


}());