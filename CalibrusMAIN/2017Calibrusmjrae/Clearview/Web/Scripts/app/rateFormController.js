(function () {

    'use strict';

    angular.module("portal")
        .controller("rateFormController", rateFormController);

    function rateFormController($log, $uibModal, $window, $location, userCtx) {

        var vm = this;

        vm.getDate = function (d) {
            $log.info("getDate");
            $log.info(d);
            if (d.length > 0) {
                return new Date(d);
            }
            else {
                return null;
            }
        };

        vm.startToday = function () {
            vm.startDt = new Date();
        };

        vm.endToday = function () {
            vm.endDt = new Date();
        };

        vm.clear = function () {
            vm.startDt = null;
        };

        vm.clear = function () {
            vm.endDt = null;
        };

        vm.dateOptions = {
            startingDay: 1
        };

        vm.openStart = function () {
            vm.startOpened = true;
        };

        vm.openEnd = function () {
            vm.endOpened = true;
        };

        vm.setStart= function(d) {
            vm.startDt = d;
        }

        vm.setEnd = function (d) {
            vm.endDt = d;
        }

        vm.open = function (rate) {

            $log.info('open');
            $log.info(rate);

            var modalInstance = $uibModal.open({
                templateUrl: 'myModalCopy.html',
                controller: 'rateCopyController',
                controllerAs: 'pop',
                backdrop: 'static',
                resolve: {
                    rate: function () {
                        return rate;
                    }
                }
            });

            modalInstance.result.then(function (data) {
                $log.info("modalInstance.result loggedInUser: " + data.loggedInUser);
                $log.info("redirect to: " + 'Rate/Copy/' + data.id);
                $window.location.href = 'Rate/Copy/' + data.id;

            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        vm.format = 'MM/dd/yyyy';
        vm.user = userCtx;
        vm.startOpened = false;
        vm.endOpened = false;
        //vm.baseUrl = "http://" + $location.host() + ":" + $location.port() + "/";
        
    }


}());

(function () {

    'use strict';

    angular.module("portal")
        .controller("rateCopyController", rateCopyController);

    function rateCopyController($log, $uibModalInstance, rate) {

        var pop = this;

        pop.rate = rate;

        pop.header = "Copy " + rate.ProgramName;
        pop.message = "Are you sure you want to make a copy of " + rate.ProgramName + " (" + rate.ProgramCode + ")?";

        pop.ok = function (u) {
            $log.info("RateID: " + rate.ProgramId);
            $uibModalInstance.close({ loggedInUser: u, id: rate.ProgramId });
        };

        pop.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

    }

}());