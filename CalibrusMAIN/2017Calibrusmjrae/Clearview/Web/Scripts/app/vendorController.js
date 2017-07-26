(function () {

    'use strict';

    angular.module("portal")
        .controller("vendorController", vendorController);

    function vendorController(portalData, $log, $location, $filter, $uibModal) {

        var vm = this;

        vm.pageChanged = function () {
            $log.info("current page: " + vm.currentPage);
            var start = (vm.currentPage - 1) * vm.pageSize;
            var end = start + vm.pageSize;
            vm.vendorPage = vm.filteredVendors.slice(start, end);
        }

        var getvendors = function () {
            vm.loadingVendors = true;
            portalData.getVendors(false).then(onComplete, onError);
        };

        var onComplete = function (data) {
            vm.loadingVendors = false;
            vm.vendors = data;
            vm.filterVendors();
        };

        var onError = function (reason) {
            vm.loadingVendors = false;
            vm.error = "Error getting vendor list";
        };

        var onStatusUpdate = function (reason) {
            $log.info("onStatusUpdate: success");
            $log.info("onStatusUpdate: Before Getting vendors for refresh");
            getvendors();
            $log.info("onStatusUpdate: After Getting vendors for refresh");

            if (vm.filter.length > 0) {
                $log.info("onStatusUpdate: filter vendors for refresh");
                $log.info("onStatusUpdate: filter = " + vm.filter);
                vm.filterVendors();
            }

        };

        var onStatusUpdateError = function (reason) {
            vm.error = "Error updating user status";
        };

        vm.filterVendors = function () {
            vm.filteredVendors = $filter('filter')(vm.vendors, function (item, index) {
                if (vm.showInactive == item.IsActive) {
                    return false;
                }
                if (item.VendorName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    item.VendorNumber.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) {
                    return true;
                }
                return false;
            });
            vm.totalItems = vm.filteredVendors.length;
            vm.vendorPage = vm.filteredVendors.slice(0, vm.pageSize);
            vm.currentPage = 1;
        };

        vm.open = function (v) {

            $log.info('open');
            $log.info(v);



            var modalInstance = $uibModal.open({
                templateUrl: 'myVendorModalContent.html',
                controller: 'vendorLogController',
                controllerAs: 'pop',
                backdrop: 'static',
                show: true,
                resolve: {
                    vendor: function () {
                        return v;
                    }
                }
            });

            modalInstance.result.then(function (data) {
                $log.info("modalInstance.result  vendorId: " + data.Id);
                $log.info("modalInstance.result loggedInUser: " + data.loggedInUser);
                portalData.updateVendorStatus(data.vendorId, data.loggedInUser).then(onStatusUpdate, onStatusUpdateError);
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        getvendors();

        vm.vendors;
        vm.vendorPage;
        vm.currentPage;
        
        if ($location.absUrl().match('(.)+(Index)(/)?')) {
            vm.pageSize = 25;
        } else {
            vm.pageSize = 5;
        }

        vm.totalItems;
        vm.filter = "";
        vm.showInactive = false;

    }

}());


(function () {

    'use strict';

    angular.module("portal")
        .controller("vendorLogController", vendorLogController);

    function vendorLogController(portalData, $log, $uibModalInstance, vendor) {

        var pop = this;

        pop.vendor = vendor;

        pop.header = vendor.IsActive ? 'Inactivate ' : 'Reactivate ';
        pop.header += vendor.VendorName + '?';

        pop.ok = function (u) {

            $uibModalInstance.close({ vendorId: pop.vendor.Id, loggedInUser: u });
        };

        pop.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

    }

}());