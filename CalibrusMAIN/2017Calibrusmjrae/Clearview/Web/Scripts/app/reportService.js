(function() {

    'use strict';

    angular.module('portal')
        .factory('reportService', reportService);
    
    function reportService($http, $log) {
        
        var service = {
            getReport: getReport,
            getDispositions: getDispositions,
            getVerifiedChartSummary: getVerifiedChartSummary,
            getTopVendors: getTopVendors,
            getTopOffices: getTopOffices,
            getTopUsers: getTopUsers
        }
        return service;

        /**************************************************************/

        function getReport(searchCriteria) {

            return $http.post('/api/report/calls/', searchCriteria)
                .then(function(response) {
                    return response.data;
                })
                .catch(function (error) {
                    $log.error('getReport error: ' + JSON.stringify(error));
                    throw(error);
                });
        }

        function getDispositions() {
            return $http.get('/api/report/dispositions/')
                .then(function(response) {
                    return response.data;
                })
                .catch(function (error) {
                    $log.error('getDispositions error: ' + JSON.stringify(error));
                    throw (error);
                });
        }

        function getVerifiedChartSummary(range, vendorId, officeId) {

            return $http.get('/api/verifiedchart/' + range + '/' + vendorId + '/' + officeId + '/')
                .then(function (response) {
             
                    return response.data;
                });
        }

        function getTopVendors(range, vendorId) {

            return $http.get('/api/topvendors/' + range + '/' + vendorId + '/')
                .then(function (response) {
                    return response.data;
                });
        }

        function getTopOffices(range, vendorId, officeId) {

            return $http.get('/api/topoffices/' + range + '/' + vendorId + '/' + officeId + '/')
                .then(function (response) {
                    return response.data;
                });
        }

        function getTopUsers(range, vendorId, officeId) {

            return $http.get('/api/topusers/' + range + '/' + vendorId + '/' + officeId + '/')
                .then(function (response) {
                    //$log.info('top users: ' + JSON.stringify(response.data));
                    return response.data;
                });
        }


    }

}());