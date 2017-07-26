(function () {

    'use strict';

    angular.module("portal")
        .controller("rateFormController", rateFormController);

    function rateFormController($log, $modal, $window, $location) {

        var vm = this;

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

        vm.openStart = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

            vm.startOpened = true;
        };

        vm.openEnd = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();

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

            var modalInstance = $modal.open({
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
        //vm.baseUrl = "http://" + $location.host() + ":" + $location.port() + "/";
        
    }


}());

(function () {

    'use strict';

    angular.module("portal")
        .controller("rateCopyController", rateCopyController);

    function rateCopyController($log, $modalInstance, rate) {

        var pop = this;

        pop.rate = rate;

        pop.header = "Copy " + rate.ProgramName;
        pop.message = "Are you sure you want to make a copy of " + rate.ProgramName + " (" + rate.ProgramCode + ")?";

        pop.ok = function (u) {
            $log.info("RateID: " + rate.ProgramId);
            $modalInstance.close({ loggedInUser: u, id: rate.ProgramId });
        };

        pop.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

    }

}());