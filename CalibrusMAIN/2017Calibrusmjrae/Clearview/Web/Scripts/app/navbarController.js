(function() {

    angular.module('portal').controller('navbarController', navbarController);

    function navbarController($log, portalData, userCtx) {

        'use strict';
        var vm = this;

        var getReportList = function() {
            portalData.getReports(userCtx.securityLevel).then(function(data) {
                vm.reportList = data;
                $log.info(JSON.stringify(vm.reportList));

            }).catch(function (error)
            {
                vm.error = "Error getting report list";
            });
        };

        $log.info('navController');
        getReportList();
    };

}());