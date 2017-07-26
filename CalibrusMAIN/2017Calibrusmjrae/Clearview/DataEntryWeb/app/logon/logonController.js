(function () {
    'use strict';

    angular
        .module('app')
        .controller('logonController', logonController);

    logonController.$inject = ['$location', '$log', '$state', '$cookies', 'logonService']; 

    function logonController($location, $log, $state, $cookies, logonService) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'logonController';

        vm.logonUser = function () {

            $log.info('vm.logon');
            vm.loggingIn = true;

            if(vm.form.$invalid)
            {
                $log.info('vm.form.$invalid');
                vm.loggingIn = false;
                return;
            }

            $log.info('vm.form.$valid');
            $log.info(JSON.stringify(vm.logon));

            logonService.validateLogon(vm.logon)
            .then(function (data) {
                vm.loggingIn = false;
                $log.info(JSON.stringify(data));
                vm.hasErrors = data.hasErrors;
                vm.errorList = data.errorList;
                $log.info(JSON.stringify(data));
                $cookies.putObject('user', data.data);
                if (vm.hasErrors === false) {
                    $state.go('request', { user:data });
                }
            })


        }

        activate();

        function activate() {
            vm.logon = {};
            vm.hasErrors = false;
            vm.loggingIn = false;
            vm.errorList = [];
            vm.clearviewIdPattern = /^\d+$/;
        }
    }
})();
