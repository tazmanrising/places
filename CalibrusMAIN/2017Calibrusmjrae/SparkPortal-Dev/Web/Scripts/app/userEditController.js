(function () {

    'use strict';

    angular.module("portal")
        .controller("userEditController", userEditController);

    function userEditController(portalData, $log, $location, $window, $modal) {

        var vm = this;

        vm.Genders = ["Male", "Female"];
        vm.Gender = "";
        vm.size = "";
        vm.femaleSizes = ["XS", "S", "M", "L", "XL", "2XL"];
        vm.maleSizes = ["S", "M", "L", "XL", "2XL", "3XL", "4XL", "5XL"];
        vm.Sizes = [];
        
        vm.GetUserLogs = function () {
            portalData.getUserLogs(vm.Id).then(function (result) {
                vm.logs = result;
            })
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
                portalData.addUserNote(data.userId, data.reason, data.loggedInUser).then(onStatusUpdate, onStatusUpdateError);
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

        vm.onGenderChanged = function () {
           
                $('#ShirtSize').val('');
                vm.size = '';
                $log.info(vm.Gender);
                $log.info("onGenderChanged Gender: ", vm.Gender);
                if (vm.Gender == "Male") {
                    vm.Sizes = vm.maleSizes;

                } else {
                    vm.Sizes = vm.femaleSizes;
                }
          
        }

       
        vm.getvendors();


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