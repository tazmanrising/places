(function () {
    'use strict';

    var injectParams = ['$http', '$q', 'userService', 'addressService', 'googleService', 'calibrusclearviewRequestService'];


    var userFactory = function ($http, $q, userService, addressService, googleService, calibrusclearviewRequestService) {

        var factory = {};
        var vm = this;

       
        factory.getServiceLocation = function () {

            var zipCodeCurrent;

            var userGeoPromise = userService.getGeoposition().then(function (geoposition) {
                vm.geoposition = geoposition;
                return addressService.reverseGeocode(geoposition.coords);
            }).then(function (data) {
                vm.currentLocation = googleService.googleAddressComponentsToAddress(data.results[0]);
                //http://localhost:22995/api/getserviceablezip/60030
                zipCodeCurrent = vm.currentLocation.zip;
            });

            //TODO   change this to be in calibrus-clearview.js
            //https://clearview.calibrus.com
            //60030  works
            //
            //var serviceBase = "http://localhost:22995/api/getserviceablezip/60030"
            //var xserviceBase = "http://localhost:22995"
            //var serviceZipPromise = $http.get(serviceBase + '/api/getserviceablezip/'+ vm.currentLocation.zip);

       

            
            return userGeoPromise.then(function (zipCodeCurrent) { // *** add the argument as in the test
                //var serviceBase = "http://localhost:2295/api/getservicezip/" + zipCodeCurrent;
                return calibrusclearviewRequestService.getServiceableZip(zipCodeCurrent);
                //return $http.get(serviceBase); // *** return the promise
            }).then(function (results) { // *** move the then-callback to the outer chain
                console.log('serviceZipPromise', results);
                return results;
            }).catch(function (error) { // *** add error handling at the end of the chain
                console.log('error occurred:', error);
            });




        }


        return factory;


    };


    userFactory.$inject = injectParams;

    angular
        .module('calibrus')
        .factory('userFactory', userFactory)

}());