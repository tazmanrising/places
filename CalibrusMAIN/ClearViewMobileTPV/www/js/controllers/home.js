"use strict";

angular.module('calibrus')
  .controller('HomeCtrl', function ($scope, $ionicModal, $ionicPopup, $state, userService, mapsService, addressService, googleService, enrollmentService,userFactory) {
    var vm = this;
    //console.log('vm HomeCtrl',vm);
    
    vm.location = {};
    vm.hasCachedEnrollment = enrollmentService.hasCachedEnrollment();

    vm.user = userService.getUser();
    userService.getGeoposition().then(function (geoposition) {
      vm.geoposition = geoposition;
      return addressService.reverseGeocode(geoposition.coords).then(function (data) {
        data.geoposition = geoposition;
        return data;
      });
    }, function (err) {
      vm.location.error = err;
    }).then(function (data) {
      var result = data.results[0];

      vm.currentLocation = googleService.googleAddressComponentsToAddress(result);
      vm.mapUrl = mapsService.staticMapFromCoords(data.geoposition.coords, {
        zoom: 18,
        markers: [{
          latitude: data.geoposition.coords.latitude,
          longitude: data.geoposition.coords.longitude,
          color: 'green'
        }, {
          latitude: result.geometry.location.lat,
          longitude: result.geometry.location.lng,
          color: 'blue'
        }]
      });

    }, function (err) {
      vm.location.error = err;
    });

    vm.newEnrollment = function () {
      if (vm.hasCachedEnrollment) return $ionicPopup.confirm({
        title: 'Confirm',
        template: 'Are you sure you want to remove your existing order?'
      }).then(function (res) {
        if (res) {
          enrollmentService.resetEnrollment();
          vm.hasCachedEnrollment = enrollmentService.hasCachedEnrollment();
          vm.newEnrollment();
         userService.track();
        }
      });

      $state.go('app.data-entry.authorizedParty');
    }
  });
