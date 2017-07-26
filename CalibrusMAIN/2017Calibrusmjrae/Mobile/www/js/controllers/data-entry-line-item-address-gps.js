"use strict";

angular.module('calibrus')
  .controller('DataEntryLineItemAddressGpsCtrl', function (geolocationService, addressService, userService) {
    var vm = this;

    vm.refreshNearbyLocations = function (isPullToRefresh) {
      vm.location = {
        error: null,
        geoposition: null
      };

      vm.pullToRefresh = !!isPullToRefresh;
      vm.nearbyLocations = null;
      userService.track();

      geolocationService.getCurrentPosition().then(function (geoposition) {
        vm.location.geoposition = geoposition;
        return addressService.reverseGeocode(geoposition.coords);
      }, function (err) {
        vm.location.error = err;
      }).then(function (data) {
        vm.nearbyLocations = data.results;
      }, function (err) {
        vm.location.error = err;
      }).finally(function () {
        $scope.$broadcast('scroll.refreshComplete');
      });
    };

    vm.refreshNearbyLocations();
  });
