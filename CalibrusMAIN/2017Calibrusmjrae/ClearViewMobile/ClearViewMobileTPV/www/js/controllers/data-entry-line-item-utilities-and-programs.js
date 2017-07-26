"use strict";

angular.module('calibrus')
  .controller('DataEntryLineItemUtilitiesAndProgramsCtrl', function ($state, $ionicPopup, $filter, calibrusclearviewRequestService, enrollmentService, userService) {
    var vm = this;

    var lineItem = enrollmentService.getLineItem($state.params.lineItemIndex);

    vm.refreshUtilitiesAndPrograms = function () {
      vm.utiliesError = null;
      vm.utilitiesAndProgramsIsLoading = true;

      var user = userService.getUser();
      if (!(lineItem && lineItem.serviceLocation && lineItem.serviceLocation.state && lineItem.serviceLocation.zip)) {
        return $ionicPopup.alert({
          title: 'Missing Address!',
          template: 'This line item is missing the service address. Please choose an address.'
        }).then(function () {
          $state.go('^.address');
        });
      }

      var state = lineItem.serviceLocation.state;
      var zip = lineItem.serviceLocation.zip;

      return calibrusclearviewRequestService.getUtilityPrograms(user.vendorId, user.officeId, state, zip)
        .then(function (utilitiesAndPrograms) {
          vm.utilitiesAndPrograms = utilitiesAndPrograms;
        }, function (err) {
          vm.utiliesError = err;
        }).finally(function () {
          vm.utilitiesAndProgramsIsLoading = false;
        });
    };

    vm.refreshUtilitiesAndPrograms();

    vm.goToCurrentUtilityInformation = function () {
      if (!lineItem.services || (!(lineItem.services.electric && lineItem.services.electric.selectedProgramId) && !(lineItem.services.naturalGas && lineItem.services.naturalGas.selectedProgramId))) {
        return $ionicPopup.alert({
          title: 'Missing Service',
          template: 'You must choose at least 1 service before proceeding.'
        });
      }
      enrollmentService.saveEnrollment();
      $state.go('^.current-utilities');
    };
  });
