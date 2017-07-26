"use strict";

angular.module('calibrus')
  .service('mapsService', function (googleService) {
    var _this = this;

    this.staticMapFromCoords = function (coords, settings) {
      return _this.staticMapFromLatLng(coords.latitude, coords.longitude, settings);
    };

    this.staticMapFromLatLng = function (latitude, longitude, settings) {
      return googleService.getStaticMapUrl(latitude, longitude, settings);
    };
  });

