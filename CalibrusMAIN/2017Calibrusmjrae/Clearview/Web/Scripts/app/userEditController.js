(function () {

    'use strict';

    angular.module("portal")
        .controller("userEditController", userEditController)
        .filter('YesNo',
        function() {
            return function(text) {
                return text ? "Yes" : "No";
            }
        });

    function userEditController(portalData, $log, $location, $window, $uibModal, $filter, userCtx) {

        var vm = this;


        vm.GetUserLogs = function () {

            console.log('vm.Id', vm.Id);

            portalData.getUserLogs(vm.Id).then(function(result) {
                vm.logs = result;
            });
        }

        vm.getDate = function (d) {
            $log.info("getDate");
            $log.info(d);
            // return $filter('date')(new Date(d), "MM/dd/yyyy");
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

        vm.clear = function () {
            vm.startDt = null;
        };

        vm.dateOptions = {
            startingDay: 1
        };

        vm.openStart = function () {
            vm.startOpened = true;
        };

        vm.setStart = function (d) {
            vm.startDt = d;
        }

        vm.setEnd = function (d) {
            vm.endDt = d;
        }

        vm.getoffices = function (id) {
            portalData.getVendorOffices(id, true).then(onGetOfficesComplete, onGetOfficesError);
        };

        var onGetOfficesComplete = function(data) {
            vm.offices = data;
        }

        var onGetOfficesError = function (reason) {
            vm.error = "Error getting offices";
        };


        vm.getvendors = function () {
            portalData.getVendors(true).then(onGetVendorsComplete, onGetVendorsError);
        };

        var onGetVendorsComplete = function (data) {
            vm.vendors = data;
        }

        var onGetVendorsError = function (reason) {
            vm.error = "Error getting vendors";
        };

        var onStatusUpdate = function (reason) {
            $log.info("onStatusUpdate: success");
            $window.location.reload();
        };

        var onStatusUpdateError = function (reason) {
            vm.error = "Error updating user status";
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

        var onGetUserTypeComplete = function (data) {
            $log.info("onGetUserTypeComplete: " + JSON.stringify(data));
            vm.userType = data;
            data && data.SecurityLevel == 0 ? vm.usernameLabel = "Agent ID" : vm.usernameLabel = "Username";
            if (data && data.SecurityLevel == 0) {
                vm.password = "";
                vm.email = "";
            } 
        }

        var onGetUserTypeError = function (data) {
            vm.error = "Error getting user type";
        }

        vm.userTypeChanged = function() {
            $log.info("userTypeChanged id: " + vm.userTypeId);
            portalData.getUserType(vm.userTypeId).then(onGetUserTypeComplete, onGetUserTypeError);

        }

        vm.onVendorChanged = function () {
            $log.info("onVendorChanged id: " + vm.selectedVendor);
            if (vm.selectedVendor) {
                vm.getoffices(vm.selectedVendor);
            }

        }
        
        vm.format = 'MM/dd/yyyy';
        vm.startOpened = false;
        vm.user = userCtx;
        vm.getvendors();


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