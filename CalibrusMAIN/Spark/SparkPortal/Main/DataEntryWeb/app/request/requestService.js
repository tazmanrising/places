(function () {
    'use strict';

    angular
        .module('app')
        .factory('requestService', requestService);

    requestService.$inject = ['$http', '$log'];

    function requestService($http, $log) {
        var service = {
            getIpLocation: getIpLocation,
            getLead: getLead,
            getProgramList: getProgramList,
            getUtilityList: getUtilityList,
            getRelationshipList: getRelationshipList,
            submitRequest: submitRequest,
            getZipcodeInfo: getZipcodeInfo
        };

        return service;

        function submitRequest(request) {
            return $http.post('api/request/', request)
                .then(function (response) {
                    return response.data;
                })
        }

        function getIpLocation() {
            return $http.get('https://ipinfo.io/?token=dcd6ba675c0e70')
                .then(function (response) {
                    return response.data;
                })
        }

        function getZipcodeInfo(zip) {
            return $http.get('https://us-zipcode.api.smartystreets.com/lookup?zipcode=' + zip + '&auth-id=15148665949482306')
                .then(function (response) {
                    return response.data;
                })
        }

        function getLead(id, vendorNumber) {
            return $http.get('api/lead/' + vendorNumber + '/' + id)
                .then(function (response) {
                    return response.data;
                })
        }

        function getProgramList(utilityId, vendorId, utilityType) {
            //$log.info('api/programs/' + state + '/' + vendorId + '/' + utilityType + '/');
            return $http.get('api/programs/' + utilityId + '/' + vendorId + '/' + utilityType + '/')
                .then(function (response) {
                    return response.data;
                })
        }

        function getUtilityList(state) {
            //$log.info('api/utilities/' + state + '/');
            return $http.get('api/utilities/' + state)
                .then(function (response) {
                    return response.data;
                })
        }

        function getRelationshipList() {
            return $http.get('api/relationships/')
                .then(function (response) {
                    return response.data;
                })
        }


    }
})();