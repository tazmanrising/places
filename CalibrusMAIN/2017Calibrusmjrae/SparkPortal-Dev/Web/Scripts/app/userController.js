(function () {

    'use strict';

    angular.module("portal")
        .controller("userController", userController);

    function userController(portalData, $log, $timeout, $location, $filter, $modal, $scope, userCtx) {

        var vm = this;

        vm.pageChanged = function() {
            $log.info("current page: " + vm.currentPage);
            var start = (vm.currentPage - 1) * vm.pageSize;
            var end = start + vm.pageSize;
            vm.userPage = vm.filteredUsers.slice(start, end);
        };

        var getusers = function (active) {
            vm.loadingUsers = true;
            $log.info("Getting all users for refresh");
            portalData.getUsers(active).then(onComplete, onError);
        };


        var getvendorusers = function (id,active) {
            vm.loadingUsers = true;
            $log.info("Getting vendor users for refresh");
            portalData.getVendorUsers(id,active).then(onComplete, onError);
        };

        var getofficeusers = function (id,active) {
            vm.loadingUsers = true;
            $log.info("Getting office users for refresh");
            portalData.getOfficeUsers(id,active).then(onComplete, onError);
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
            vm.getUsersForSecurityGroup();
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
                    item.SparkId.indexOf(vm.filter) > -1 ||
                    item.AgentId.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1 ||
                    (item.Vendor.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.Office.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1) ||
                    (item.UserType.toUpperCase().indexOf(vm.filter.toUpperCase()) > -1)) {
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
                portalData.updateUserStatus(data.userId, data.reason, data.loggedInUser).then(onStatusUpdate, onStatusUpdateError);
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        vm.createNote = function (u) {

            $log.info('createNote');
            $log.info(u);

            var modalInstance = $modal.open({
                templateUrl: 'createNoteModal.html',
                controller: 'userNoteController',
                controllerAs: 'pop',
                backdrop: 'static',
                resolve: {
                    user: function () {
                        return u;
                    }
                }
            });

            modalInstance.result.then(function (data) {
                portalData.addUserNote(data.userId, data.reason, data.loggedInUser).then(onCrerateNote, onCrerateNoteError);
            }, function () {
                $log.info('Modal dismissed at: ' + new Date());
            });
        };

        var onCrerateNote = function (reason) {
            $log.info('createNote SUCCESS');
        };

        var onCrerateNoteError = function (reason) {
            vm.error = "Error adding note";
        };

        vm.getUsersForSecurityGroup = function() {
            if (vm.user.userVendorId > 0) {
                if (vm.user.userOfficeId > 0) {
                    $log.info('calling usercontroller.getofficeusers: ' + vm.activeOfficeId);
                    getofficeusers(vm.user.userOfficeId,!vm.showInactive);
                } else {
                    $log.info('calling usercontroller.getvendorusers: ' + vm.activeVendorId);
                    getvendorusers(vm.user.userVendorId,!vm.showInactive);
                }
            } else {
                $log.info('calling getusers');
                $log.info(!vm.showInactive);
                getusers(!vm.showInactive);
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
        vm.simpleNote = "";
        vm.showInactive = false;
        vm.activeVendorId;
        vm.activeOfficeId;

        vm.user = userCtx;
        vm.getUsersForSecurityGroup();
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

(function () {

    'use strict';

    angular.module("portal")
        .controller("userNoteController", userNoteController);

    function userNoteController(portalData, $log, $modalInstance, user) {

        var pop = this;

        pop.user = user;

        pop.header = 'Add note for ' + user.FirstName + ' ' + user.LastName;

        pop.ok = function (u) {

            $modalInstance.close({ userId: pop.user.UserId, reason: pop.reason, loggedInUser: u });
        };

        pop.cancel = function () {
            $modalInstance.dismiss('cancel');
        };

    }

}());