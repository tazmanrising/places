(function () {

    'use strict';

    angular.module("portal")
        .controller("rateController", rateController);

    function rateController(portalData, $log, $location, $filter, $uibModal, $window, userCtx) {

        var vm = this;

        vm.pageChanged = function () {
            $log.info("current page: " + vm.currentPage);
            var start = (vm.currentPage - 1) * vm.pageSize;
            var end = start + vm.pageSize;
            vm.ratePage = vm.filter.length > 0 ? vm.filteredRates.slice(start, end) : vm.rates.slice(start, end);
        }
        
        var getRates = function () {
            vm.loadingRates = true;
            portalData.getRates().then(onComplete, onError);
        };

        var onComplete = function (data) {
            vm.loadingRates = false;
            vm.rates = data;
            vm.filterRates();
            vm.sortRates("ProgramCode");
        };

        var onError = function (reason) {
            vm.loadingRates = false;
            vm.error = "Error getting rate list";
        };

        vm.filterRates = function () {
            vm.filteredRates = $filter('filter')(vm.rates, function (item, index) {
                if (item.ProgramName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    item.ProgramCode.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    item.State.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    (new Date(item.EffectiveStartDate).toDateString().toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (new Date(item.EffectiveEndDate).toDateString().toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.Utility && item.Utility.Name.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.Brand && item.Brand.BrandName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.Msf && item.Msf.toString().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.Etf && item.Etf.toString().indexOf(vm.filter.toUpperCase()) > -1)) {
                    return (!vm.showInactive && new Date(item.EffectiveEndDate) >= new Date() || vm.showInactive && new Date(item.EffectiveEndDate) < new Date());
                }
                return false;
            });
            vm.totalItems = vm.filteredRates.length;
            vm.ratePage = vm.filteredRates.slice(0, vm.pageSize);
            vm.currentPage = 1;
        };

        vm.sortRates = function (predicate) {            
            vm.reverse = (vm.predicate === predicate) ? !vm.reverse : false;
            vm.predicate = predicate;
            vm.filteredRates = $filter('orderBy')(vm.filteredRates, vm.predicate, vm.reverse)
            vm.totalItems = vm.filteredRates.length;
            vm.ratePage = vm.filteredRates.slice(0, vm.pageSize);
            vm.currentPage = 1;
        };
        
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
                $log.info("redirect to: " + '/Rate/Copy/' + data.id);
                $window.location.href = '/Rate/Copy/' + data.id;

            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };
        
        getRates();

        vm.rates;
        vm.ratePage;        
        vm.currentPage;

        if ($location.absUrl().match('(.)+(Index)(/)?')) {
            vm.pageSize = 25;
        } else {
            vm.pageSize = 5;
        }

        vm.totalItems;
        vm.filter = "";
        vm.showInactive = false;
        vm.reverse = true;
        vm.predicate = "ProgramCode";
        vm.user = userCtx;
        
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