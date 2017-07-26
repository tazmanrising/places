(function () {

    'use strict';

    angular.module("portal")
        .controller("securityController", securityController);

    function securityController($log, $scope, userCtx) {

        var vm = this;
        vm.user = userCtx;

    }

}());