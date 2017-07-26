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
            getMainClone: getMainClone,
            getProgramList: getProgramList,
            getUtilityList: getUtilityList,
            getRelationshipList: getRelationshipList,
            submitRequest: submitRequest,
            getZipcodeInfo: getZipcodeInfo,
            getTitleList: getTitleList,
            getProgramUtility: getProgramUtility
        };

        return service;

        function getProgramUtility(vendorid, officeid, state, zip, creditcheck,premisetype) {
            return $http.get('api/getutilityprograms/' + vendorid + '/' + officeid + '/' + state + '/' + zip + '/' + creditcheck + '/' + premisetype)
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

        function getZipcodeInfo(zip) {
            var req = {
                method: 'GET',
                url: 'https://us-zipcode.api.smartystreets.com/lookup?zipcode=' + zip + '&auth-id=30044805091098764',
                headers: {
                    'Auth-Token': undefined
                }
            }
            return $http(req)
                .then(function(response) {
                    return response.data;
                });
        }

        function getLead(id, vendorNumber) {
            return $http.get('api/lead/' + vendorNumber + '/' + id)
                .then(function(response) {
                    return response.data;
                });
        }

        function getMainClone(id) {
            return $http.get('api/main/' + id)
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