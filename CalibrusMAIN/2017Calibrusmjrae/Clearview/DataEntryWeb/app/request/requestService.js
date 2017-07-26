(function () {
    'use strict';

    angular
        .module('app')
        .factory('requestService', requestService);

    requestService.$inject = ['$http', '$log'];

    function requestService($http, $log) {
        var service = {
            getIpLocation: getIpLocation,
            getProgramList: getProgramList,
            getUtilityList: getUtilityList,
            getRelationshipList: getRelationshipList,
            submitRequest: submitRequest,
            getLead: getLead
        };

        return service;

        function getLead(id, vendorNumber) {
            return $http.get('api/lead/' + vendorNumber + '/' + id)
                .then(function (response) {
                    return response.data;
                })
        }

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

        function getProgramList(utilityId, vendorId, utilityType, accountType) {
            //$log.info('api/programs/' + state + '/' + vendorId + '/' + utilityType + '/');
            return $http.get('api/programs/' + utilityId + '/' + vendorId + '/' + utilityType + '/' + accountType)
                .then(function (response) {
                    return response.data;
                })
        }

        function getUtilityList(vendorId, utilityType, accountType, state) {
            //$log.info('api/utilities/' + state + '/');
            return $http.get('api/utilities/' + vendorId + '/' + utilityType + '/' + accountType + '/' + state)
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

        function getTitleList() {
            return $http.get('api/titles/')
                .then(function (response) {
                    return response.data;
                })
        }


    }
})();