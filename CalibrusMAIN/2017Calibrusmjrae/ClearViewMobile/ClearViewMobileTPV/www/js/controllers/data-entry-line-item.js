"use strict";

angular.module('calibrus')
  .controller('DataEntryLineItemCtrl', function ($rootScope, $window, $state, $ionicPopup, addressService, googleService, enrollmentService, calibrusclearviewRequestService, formValidationService, userCache) {
    var vm = this;

    $rootScope.notifyIonicGoingBack = function () {
      vm.lineItem.serviceLocation.address2 = "";
    }


    vm.lineItem = enrollmentService.getLineItem($state.params.lineItemIndex);

    calibrusclearviewRequestService.getRelationships().then(function (relationships) {
      vm.relationships = relationships;
    });

    // ========== Service Location IE the initial address of service ==========
    vm.setServiceLocation = function (location, next) {
      vm.lineItem.serviceLocation = googleService.googleAddressComponentsToAddress(location);
      enrollmentService.saveEnrollment();
      if (next) $state.go('^.utilities-and-programs');
    };

    vm.removeServiceLocation = function () {
      vm.lineItem.serviceLocation = null;
      enrollmentService.saveEnrollment();
      $state.go('^.address-gps', { location: 'replace' });
    };

    // ========== Address Manual ==========
    vm.manualLocation = {};
    vm.buildingType = "APT";


    vm.showAddress = function (secondAddress) {
      vm.secondAddress = !secondAddress;
    };

    vm.processAddress = function (formCtrl) {
      //COULD USE userCache (angular-cache w/ localStorage ) 
      var order = userCache.get('order');
      var orderAddress1 = order.lineItems[0].serviceLocation.address1;
      var city =  order.lineItems[0].serviceLocation.city;
      var state =  order.lineItems[0].serviceLocation.state;
      var zip =  order.lineItems[0].serviceLocation.zip;
      console.log('order',order);
      
      //Hydrate a sessionStorage
      //$window.sessionStorage.newAddress = orderAddress1 + " " + vm.lineItem.serviceLocation.buildingType + " " + vm.lineItem.serviceLocation.address2;

      vm.lineItem.serviceLocation.address2 = vm.buildingType + " " + vm.lineItem.serviceLocation.address2;
      
      vm.lineItem.serviceLocation.formattedAddress = orderAddress1 + " " + vm.lineItem.serviceLocation.address2 + ", " + city + ", " + state + " " + zip; 

      //vm.lineItem.serviceLocation.formattedAddress = "blah";
      
      //console.log(' vm.lineItem.serviceLocation.address2', vm.lineItem.serviceLocation.address2);
      //console.log('vm.lineItem.buildingType',vm.buildingType);
      console.log(' vm.lineItem.serviceLocation.formattedAddress', vm.lineItem.serviceLocation.formattedAddress);

      if (vm.lineItem.serviceLocation) return $state.go('^.^.utilities-and-programs');
      formValidationService.validateForm(formCtrl).then(function () {
        console.log('in form validation');
        vm.setServiceLocation(vm.manualLocation);
      });
    };

    // TODO : Add a method to use google to suggest address.

    vm.getCityStateFromZip = function (zip, isValid) {
      if (!isValid) return;

      vm.zipInfo = null;

      addressService.zipcodeInfo(zip).then(function (zipInfo) {
        vm.zipInfo = zipInfo;

        vm.manualLocation.city = vm.zipInfo.city;
        vm.manualLocation.state = vm.zipInfo.state;
      });
    };

    // ========== Billing Location ==========
    vm.setBillingLocation = function (location, utilityType) {
      vm.lineItem.services[utilityType].currentService.billingLocation = googleService.googleAddressComponentsToAddress(location);
      enrollmentService.saveEnrollment();
    };

    // ========== Utility and Program ==========
    vm.setUtilityAndProgram = function (serviceType, utilityAndProgram, serviceProvider) {

      vm.lineItem.services[serviceType].utilityAndProgram = utilityAndProgram;
      vm.lineItem.services[serviceType].serviceProvider = serviceProvider;
      vm.lineItem.services[serviceType].currentService = angular.copy(enrollmentService.defaults.currentService);
      enrollmentService.saveEnrollment();
    };

    vm.clearUtilityAndProgram = function (serviceType) {
      $ionicPopup.confirm({
        title: 'Confirm',
        template: 'Are you sure you want to remove this service?'
      }).then(function (res) {
        if (res) {
          vm.lineItem.services[serviceType] = null;
          enrollmentService.saveEnrollment();
        }
      });
    };
  });
