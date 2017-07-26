(function () {
    'use strict';

    angular
        .module('app')
        .factory('logonService', logonService);

    logonService.$inject = ['$http', '$log'];

    function logonService($http, $log) {
        var service = {
            validateLogon: validateLogon,
            getUser: getUser
        };

        var user;
        
        return service;

        function getUser() {
            return user;
        }

        function validateLogon(l) {

            $log.info('logonService.validateLogon');

            return $http.post("/api/dataentry/logon", l)
                .then(function (response) {
                    user = response.data.data;
                    return response.data;
                })
                .catch(function(error){
                    return error;
                })
        }
    }
})();