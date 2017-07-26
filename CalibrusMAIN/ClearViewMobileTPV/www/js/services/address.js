"use strict";

angular.module('calibrus')
  .service('addressService', function (googleService, ipInfoService, smartyStreetsService) {
    var _this = this;

    this.reverseGeocode = function (coords) {
      return googleService.getReverseGeocode(coords);
    };

    this.ipInfo = function () {
      return ipInfoService.getIpInfo();
    };

    this.zipcodeInfo = function (zipcode) {
      smartyStreetsService.getZipcodeInfo(zipcode);
    };
  });
