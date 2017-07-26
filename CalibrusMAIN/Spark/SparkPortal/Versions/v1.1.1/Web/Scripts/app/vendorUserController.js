(function () {

    'use strict';

    angular.module("portal")
        .controller("vendorUserController", vendorUserController);

    function vendorUserController(portalData, $log, $location, $filter, $modal) {

        var vm = this;

        vm.pageChanged = function () {
            $log.info("current page: " + vm.currentPage);
            var start = (vm.currentPage - 1) * vm.pageSize;
            var end = start + vm.pageSize;
            vm.userPage = vm.filter.length > 0 ? vm.filteredUsers.slice(start, end) : vm.users.slice(start, end);
        }

        var getusers = function () {
            $log.info("Getting users for refresh");
            portalData.getOfficeUsers(vm.officeId).then(onComplete, onError);
        };

        var onComplete = function (data) {
            vm.users = data;
            vm.totalItems = data.length;
            vm.userPage = vm.users.slice(0, vm.pageSize);
            vm.currentPage = 1;
        };

        var onError = function (reason) {
            vm.error = "Error getting user list";
        };

        var onStatusUpdate = function (reason) {
            $log.info("onStatusUpdate: success");
            $log.info("onStatusUpdate: Before Getting users for refresh");
            getusers();
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
            $log.info('filterUsers: ' + vm.filter);
            vm.filteredUsers = $filter('filter')(vm.users, function (item, index) {
                if (item.FirstName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    item.LastName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    item.AgentId.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    (item.Vendor && item.Vendor.VendorName.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.Vendor && item.Vendor.VendorNumber.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1)) {
                    $log.info('filterUsers $filter: ' + item.LastName);
                    return true;
                }
                return false;
            });
            $log.info('vm.filteredUsers.length : ' + vm.filteredUsers.length);
            vm.totalItems = vm.filteredUsers.length;
            vm.userPage = vm.filteredUsers.slice(0, vm.pageSize);
            vm.currentPage = 1;
        };

        vm.open = function (u) {

            $log.info('open');
            $log.info(u);

            var modalInstance = $modal.open({
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

        vm.officeId = $location.absUrl().split('/');
        vm.officeId = vm.officeId.pop();
        getusers(vm.officeId);

        vm.users;
        vm.userPage;
        vm.currentPage;

        vm.pageSize = 5;

        vm.totalItems;
        vm.filter = "";
    }

}());

(function () {

    'use strict';

    angular.module("portal")
        .controller("userLogController", userLogController);

    function userLogController(portalData, $log, $modalInstance, user) {

        var pop = this;

        pop.user = user;

        pop.header = user.IsActive ? 'Inactivate ' : 'Reactivate ';
        pop.header += user.FirstName + ' ' + user.LastName + '?';

        pop.ok = function (u) {

            $modalInstance.close({ userId: pop.user.UserId, reason: pop.reason, loggedInUser: u });
        };

        pop.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

    }

}());