"use strict";

angular.module('calibrus')
  .controller('AgentInfoCtrl', function (userService, addressService, googleService) {
    var vm = this;

    vm.user = userService.getUser();
    userService.getGeoposition().then(function (geoposition) {
      vm.geoposition = geoposition;
      return addressService.reverseGeocode(geoposition.coords);
    }).then(function (data) {
      vm.currentLocation = googleService.googleAddressComponentsToAddress(data.results[0]);
    });
    vm.ipInfo = userService.getIpInfo();
  });
