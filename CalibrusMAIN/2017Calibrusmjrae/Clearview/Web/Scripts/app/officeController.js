(function () {

    'use strict';

    angular.module("portal")
        .controller("officeController", officeController);

    function officeController(portalData, $log, $location, $filter, $uibModal, $scope) {

        var vm = this;

        vm.pageChanged = function () {
            $log.info("current page: " + vm.currentPage);
            var start = (vm.currentPage - 1) * vm.pageSize;
            var end = start + vm.pageSize;
            vm.officePage = vm.filteredOffices.slice(start, end);
        }

        var getoffices = function () {
            $log.info("getoffices");
            vm.loadingOffices = true;
            portalData.getOffices(false).then(onComplete, onError);
        };

        var getvendoroffices = function (vendorId) {
            $log.info("getvendoroffices: " + vendorId);
            vm.loadingOffices = true;
            portalData.getVendorOffices(vendorId, false).then(onComplete, onError);
        };

        var onComplete = function (data) {
            vm.loadingOffices = false;
            vm.offices = data;
            vm.filterOffices();
        };

        var onError = function (reason) {
            vm.loadingOffices = false;
            vm.error = "Error getting office list";
        };

        var onStatusUpdate = function (reason) {
            $log.info("onStatusUpdate: success");
            $log.info("onStatusUpdate: Before Getting offices for refresh");
            getoffices();
            $log.info("onStatusUpdate: After Getting offices for refresh");

            if (vm.filter.length > 0) {
                $log.info("onStatusUpdate: filter offices for refresh");
                $log.info("onStatusUpdate: filter = " + vm.filter);
                vm.filterOffices();
            }

        };

        var onStatusUpdateError = function (reason) {
            vm.error = "Error updating user status";
        };

        vm.filterOffices = function () {
            
            vm.filteredOffices = $filter('filter')(vm.offices, function (item, index) {
                if (vm.showInactive == item.IsActive) {
                    return false;
                }
                if (item.OfficeName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    item.OfficeEmail.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    item.VendorName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) {
                    return true;
                }
                return false;
            });
            vm.totalItems = vm.filteredOffices.length;
            vm.officePage = vm.filteredOffices.slice(0, vm.pageSize);
            vm.currentPage = 1;
        };

        vm.open = function (o) {

            $log.info('open');
            $log.info(o);

            var modalInstance = $uibModal.open({
                templateUrl: 'myOfficeModalContent.html',
                controller: 'officeLogController',
                controllerAs: 'pop',
                backdrop: 'static',
                resolve: {
                    office: function () {
                        return o;
                    }
                }
            });

            modalInstance.result.then(function (data) {
                $log.info("modalInstance.result  officeId: " + data.Id);
                $log.info("modalInstance.result loggedInUser: " + data.loggedInUser);
                portalData.updateOfficeStatus(data.officeId, data.loggedInUser).then(onStatusUpdate, onStatusUpdateError);



            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        $scope.$watch(vm.activeVendorId, function () {
            $log.info('vm.activeVendorId IN WATCH: ' + vm.activeVendorId);

            if (vm.activeVendorId > 0) {
                $log.info('calling getvendoroffices: ' + vm.activeVendorId);
                getvendoroffices(vm.activeVendorId);
            } else {
                $log.info('calling getoffices');
                getoffices();
            }
        });

        vm.offices;
        vm.officePage;
        vm.currentPage;

        if ($location.absUrl().match('(.)+(Index)(/)?')) {
            vm.pageSize = 25;
        } else {
            vm.pageSize = 5;
        }

        vm.totalItems;
        vm.filter = "";
        vm.showInactive = false;
        vm.activeVendorId;

    }

}());

(function () {

    'use strict';

    angular.module("portal")
        .controller("officeLogController", officeLogController);

    function officeLogController(portalData, $log, $uibModalInstance, office) {

        var pop = this;

        pop.office = office;

        pop.header = office.IsActive ? 'Inactivate ' : 'Reactivate ';
        pop.header += office.OfficeName + '?';

        pop.ok = function (u) {

            $uibModalInstance.close({ officeId: pop.office.Id, loggedInUser: u });
        };

        pop.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

    }

}());