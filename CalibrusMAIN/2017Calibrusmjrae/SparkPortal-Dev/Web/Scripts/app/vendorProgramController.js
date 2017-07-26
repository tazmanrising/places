(function () {

    'use strict';

    angular.module("portal")
        .controller("vendorProgramController", vendorProgramController);

    function vendorProgramController(portalData, $log, $location, $filter) {

        var vm = this;

        vm.pageChanged = function () {
            $log.info("current page: " + vm.currentPage);
            var start = (vm.currentPage - 1) * vm.pageSize;
            var end = start + vm.pageSize;
            vm.ratePage = vm.filter.length > 0 ? vm.filteredRates.slice(start, end) : vm.rates.slice(start, end);
        }

        var getRates = function () {
            $log.info("Getting programs for refresh");
            portalData.getVendorPrograms(vm.vendorId).then(onComplete, onError);
        };

        var onComplete = function (data) {
            vm.loadingRates = false;
            vm.rates = data;
            vm.filterRates();
            vm.sortRates("ProgramCode");
        };

        var onError = function (reason) {
            vm.error = "Error getting program list";
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
        
        $log.info("In vendorprogramController");

        vm.vendorId = $location.absUrl().split('/');
        vm.vendorId = vm.vendorId.pop();
        getRates(vm.vendorId);

        vm.rates;
        vm.ratePage;
        vm.currentPage;

        vm.pageSize = 10;

        vm.totalItems;
        vm.filter = "";
        vm.showInactive = false;
        vm.reverse = true;
        vm.predicate = "ProgramCode";

    }

}());
