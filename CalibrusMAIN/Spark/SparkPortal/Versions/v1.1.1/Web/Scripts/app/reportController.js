(function() {

    'use strict';

    angular.module("portal")
        .controller("reportController", reportController)
        .controller("startDateController", startDateController)
        .controller("endDateController", endDateController);

    function reportController(reportService, portalData, userCtx, $log, $scope, $anchorScroll, $filter) {

        var vm = this;

        var getDispositions = function () {
            reportService.getDispositions().then(function(data) {
                vm.dispositions = data;
            }, function(reason) {
                vm.error = "Error getting disposition list";
            });
        };

        var getVendors = function () {
            portalData.getVendors(false).then(function (data) {
                vm.vendors = data;
            }, function (reason) {
                vm.error = "Error getting vendor list";
            });
        };

        vm.getOffices = function (id) {
            $log.info('vm.getOffices(): ' + id);
            portalData.getVendorOffices(id, false).then(function (data) {
                vm.offices = data;
            }, function (reason) {
                vm.error = "Error getting office list";
            });
        };

        vm.getResults = function(isValid) {

            vm.submitted = true;
            

            if (!isValid) {
                $log.info("Form invalid!");
                $anchorScroll();
                return;
            }

            if (vm.user.userVendorId && vm.user.userVendorId > 0) {
                vm.search.vendorId = vm.user.userVendorId;
            }
            if (vm.user.userOfficeId && vm.user.userOfficeId > 0) {
                vm.search.officeId = vm.user.userOfficeId;
            }

            vm.resultsLoading = true;

            reportService.getReport(vm.search).then(function (data) {
                vm.results = data;
                vm.filteredResults = data;
                vm.resultsLoading = false;
            }, function (reason) {
                vm.resultsLoading = false;
                vm.error = "Error getting results";
            });
        };

        vm.filterResults = function () {
            vm.filteredResults = $filter('filter')(vm.results, function (item, index) {
                if (item.MainId.toString().indexOf(vm.filter) > -1 ||
                    (item.ConcernCode != null && item.ConcernCode.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.AuthorizationFirstName != null && item.AuthorizationFirstName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.AuthorizationLastName != null && item.AuthorizationLastName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.AccountFirstName != null && item.AccountFirstName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.AccountLastName != null && item.AccountLastName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.SalesState != null && item.SalesState.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.Btn != null && item.Btn.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.User != null && item.User.Vendor != null && item.User.Vendor.VendorName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.User != null && item.User.AgentId.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.TpvAgentId != null && item.TpvAgentId.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    item.Concern.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) {
                    return true;
                }
                return false;
            });
        };

        vm.newSearch = function () {
            if ($scope.callForm) {
                $scope.callForm.$setPristine();
                $scope.callForm.$setUntouched();
            }
            
            vm.results = undefined;
            vm.search = undefined;

            vm.search = {
                recordId: '',
                verificationCode: '',
                phoneNumber: '',
                vendorAgentId: '',
                tpvAgentId: '',
                accountNumber: '',
                startDate: undefined,
                endDate: undefined,
                disposition: undefined,
                vendorId: undefined,
                officeId: undefined
            }
        };

        ////datepickers
        //vm.dateOptions = {
        //    startingDay: 1
        //};

        //vm.format = 'MM/dd/yyyy';

        //vm.startOpen = function ($event) {
        //    $event.preventDefault();
        //    $event.stopPropagation();
        //    vm.startOpened = true;
        //};

        //vm.endOpen = function ($event) {
        //    $event.preventDefault();
        //    $event.stopPropagation();
        //    vm.endOpened = true;
        //};

        //vm.toggleEndMin = function() {
        //    if (vm.search.startDate) {
        //        vm.minEnd = vm.search.startDate;
        //    }
        //}
         
        vm.resultsLoading = false
        vm.user = userCtx;
        vm.newSearch();
        getDispositions();
        getVendors();

    }


    function startDateController(reportService, portalData, userCtx, $log, $scope, $anchorScroll) {
        var vm = this;
        reportController.apply(vm, arguments);

        vm.today = function () {
            vm.search.startDate = new Date().setHours(0, 0, 0, 0);
        };

        $scope.clear = function () {
            $log.info("startDateController.clear()");
            vm.search.startDate = undefined;
        };

        vm.dateOptions = {
            startingDay: 1,

        };

        vm.open = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
            vm.today();
            vm.opened = true;
        };

        vm.set = function (d) {
            vm.startDate = d;
        }

        vm.format = 'MM/dd/yyyy';
        
    }

    function endDateController(reportService, portalData, userCtx, $log, $scope, $anchorScroll) {
        var vm = this;
        reportController.apply(vm, arguments);
        
        $log.info('endVM: ' + JSON.stringify(vm));

        vm.today = function () {
            
            vm.search.endDate = new Date().setHours(0, 0, 0, 0);
        };

        $scope.clear = function () {
            $log.info("endDateController.clear()");
            vm.search.endDate = undefined;
        };

        vm.dateOptions = {
            startingDay: 1,
            initDate: new Date()
        };

        vm.open = function ($event) {
            $event.preventDefault();
            $event.stopPropagation();
           
            vm.opened = true;
        };

        vm.set = function (d) {
            vm.endDate = d;
        }

        vm.format = 'MM/dd/yyyy';
        vm.today();

    }

}());