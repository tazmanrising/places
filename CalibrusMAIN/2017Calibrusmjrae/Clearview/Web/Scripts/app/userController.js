(function () {

    'use strict';

    angular.module("portal")
        .controller("userController", userController);

    function userController(portalData, $log, $location, $filter, $uibModal, $scope, userCtx) {

        var vm = this;

        vm.pageChanged = function() {
            $log.info("current page: " + vm.currentPage);
            var start = (vm.currentPage - 1) * vm.pageSize;
            var end = start + vm.pageSize;
            vm.userPage = vm.filteredUsers.slice(start, end);
        };

        var getusers = function (getAdmins) {
            vm.loadingUsers = true;
            $log.info("Getting all users for refresh");
            portalData.getUsers(getAdmins).then(onComplete, onError);
        };

        var getvendorusers = function (id) {
            vm.loadingUsers = true;
            $log.info("Getting vendor users for refresh");
            portalData.getVendorUsers(id).then(onComplete, onError);
        };

        var getofficeusers = function (id) {
            vm.loadingUsers = true;
            $log.info("Getting office users for refresh");
            portalData.getOfficeUsers(id).then(onComplete, onError);
        };

        var onComplete = function (data) {
            vm.loadingUsers = false;
            vm.users = data;
            vm.filterUsers();
            vm.sortUsers('LastName');
        };

        var onError = function (reason) {
            vm.loadingUsers = false;
            vm.error = "Error getting user list";
        };

        var onStatusUpdate = function(reason) {
            $log.info("onStatusUpdate: success");
            $log.info("onStatusUpdate: Before Getting users for refresh");
            getUsersForSecurityGroup();
            $log.info("onStatusUpdate: After Getting users for refresh");

            if (vm.filter.length > 0) {
                $log.info("onStatusUpdate: filter users for refresh");
                $log.info("onStatusUpdate: filter = " + vm.filter);
                vm.filterUsers();
            }

        };

        var onStatusUpdateError = function (reason) {
            vm.error = "Error updating user status";
        };

        vm.filterUsers = function () {
            vm.filteredUsers = $filter('filter')(vm.users, function (item, index) {
                if (vm.showInactive == item.IsActive) {
                    return false;
                }
                if (item.FirstName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    item.LastName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    item.AgentId.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    (item.Vendor && item.Vendor.VendorName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.Vendor && item.Vendor.VendorNumber.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.Office && item.Office.OfficeName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.UserType && item.UserType.UserTypeName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1)) {
                    return true;
                }
                return false;
            });
            vm.totalItems = vm.filteredUsers.length;
            vm.userPage = vm.filteredUsers.slice(0, vm.pageSize);
            vm.currentPage = 1;
        };

        vm.sortUsers = function (predicate) {
            vm.reverse = (vm.predicate === predicate) ? !vm.reverse : false;
            vm.predicate = predicate;
            vm.filteredUsers = $filter('orderBy')(vm.filteredUsers, vm.predicate, vm.reverse)
            vm.totalItems = vm.filteredUsers.length;
            vm.userPage = vm.filteredUsers.slice(0, vm.pageSize);
            vm.currentPage = 1;
        };

        vm.open = function (u) {

            $log.info('open');
            $log.info(u);

            var modalInstance = $uibModal.open({
                templateUrl: 'myModalContent.html',
                controller: 'userLogController',
                controllerAs: 'pop',
                backdrop: 'static',
                resolve: {
                    user: function () {
                        return u;
                    }
                }
            });

            modalInstance.result.then(function (data) {
                $log.info("modalInstance.result userId: " + data.userId);
                $log.info("modalInstance.result reason: " + data.reason);
                $log.info("modalInstance.result loggedInUser: " + data.loggedInUser);
                portalData.updateUserStatus(data.userId, data.reason, data.loggedInUser).then(onStatusUpdate, onStatusUpdateError);
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        var getUsersForSecurityGroup = function() {
            if (vm.user.userVendorId > 0) {
                if (vm.user.userOfficeId > 0) {
                    $log.info('calling usercontroller.getofficeusers: ' + vm.activeOfficeId);
                    getofficeusers(vm.user.userOfficeId);
                } else {
                    $log.info('calling usercontroller.getvendorusers: ' + vm.activeVendorId);
                    getvendorusers(vm.user.userVendorId);
                }
            } else {
                $log.info('is qa admin? ' + userCtx.isQaAdmin);
                
                if (userCtx.isQaAdmin=='True') {
                    $log.info('calling getusers. getAdmins? false');
                    getusers(false);
                }
                else
                {
                    $log.info('calling getusers. getAdmins? true');
                    getusers(true);
                }
            }
        }

        vm.users;
        vm.userPage;
        vm.currentPage;

        if ($location.absUrl().match('(.)+(Index)(/)?')) {
            vm.pageSize = 25;
        } else {
            vm.pageSize = 5;
        }

        vm.reverse = true;
        vm.predicate = "LastName";
        
        vm.totalItems;
        vm.filter = "";
        vm.showInactive = false;
        vm.activeVendorId;
        vm.activeOfficeId;

        vm.user = userCtx;
        getUsersForSecurityGroup();
    }

}());

(function () {

    'use strict';

    angular.module("portal")
        .controller("userLogController", userLogController);

    function userLogController(portalData, $log, $uibModalInstance, user) {

        var pop = this;

        pop.user = user;

        pop.header = user.IsActive ? 'Inactivate ' : 'Reactivate ';
        pop.header += user.FirstName + ' ' + user.LastName + '?';

        pop.ok = function (u) {

            $uibModalInstance.close({ userId: pop.user.UserId, reason: pop.reason, loggedInUser: u });
        };

        pop.cancel = function () {
            $uibModalInstance.dismiss('cancel');
        };

    }

}());