"use strict";

angular.module('calibrus')
  .controller('LogonCtrl', function ($ionicLoading, $ionicPopup, $state, userService, geolocationService) {
    var vm = this;

    vm.credentials = {};

    vm.processLocation = function () {
      vm.location = {
        error: null,
        geoposition: null
      };

      geolocationService.getCurrentPosition().then(function (geoposition) {
        vm.location.geoposition = geoposition;
      }, function (err) {
        vm.location.error = err;
      });
    };

    vm.processLocation();

    // from lat,lng we get zipcode. From zipcode we get city, state, zip

    vm.sparkLogon = function () {
      $ionicLoading.show();

      userService.sparkLogon(vm.credentials)
        .then(function () {
          $state.go('app.home');
        }, function (errorList) {
          $ionicPopup.alert({
            title: 'On no!',
            template: Array.isArray(errorList) ? errorList.join() : "Something went wrong"
          });
        })
        .finally(function () {
          $ionicLoading.hide();
          userService.track();
        });
    };
  });
