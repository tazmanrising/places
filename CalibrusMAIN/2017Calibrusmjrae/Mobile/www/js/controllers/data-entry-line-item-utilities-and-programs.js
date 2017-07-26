"use strict";

angular.module('calibrus')
  .controller('DataEntryLineItemUtilitiesAndProgramsCtrl', function ($window, $state, $ionicPopup, $filter, calibrusSparkRequestService, enrollmentService, userService) {
    var vm = this;

    // write out localstorage and the message 
    //var data = $window.localStorage['my-data'];

    //console.log(data);


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
       
      vm.authorizedParty = enrollmentService.getAuthorizedParty();
      
      
      var creditCheck = true;
      if(vm.authorizedParty.creditCheck == false){
        creditCheck = false;
      }

      console.log('creditCheck',creditCheck);
      console.log('user',user);
      console.log('state',state);
      console.log('zip',zip);
      
      return calibrusSparkRequestService.getUtilityPrograms(user.vendorId, user.officeId, state, zip, creditCheck)
        .then(function (utilitiesAndPrograms) {
          console.log('utilitiesAndPrograms',utilitiesAndPrograms);
          if(utilitiesAndPrograms[0] == null){
            console.log('empty utilities');
            $ionicPopup.alert({
              title: 'No Utilities Found',
              template: 'There were no utilities found. Please go back and try again.' 
            });

          }
          vm.utilitiesAndPrograms = utilitiesAndPrograms;
        }, function (err) {
          console.log('get utilities error', err);
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
