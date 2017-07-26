"use strict";

angular.module('calibrus')
  .service('geolocationService', function ($q) {
    var _this = this;

    var options = {
      enableHighAccuracy: true,
      timeout: 5000,
      maximumAge: 0
    };

    this.hasGeolocation = function () {
      return !!navigator.geolocation;
    };

    this.getCurrentPosition = function () {
      if (!_this.hasGeolocation()) {
        return $q.reject({
          code: 0,
          message: 'Geolocation is not supported by this browser'
        });
      }

      var defer = $q.defer();

      navigator.geolocation.getCurrentPosition(function (pos) {
        return defer.resolve(pos);
      }, function (err) {
        // 0 : SERVICE_UNAVAILABLE (custom)
        // https://developer.mozilla.org/en-US/docs/Web/API/PositionError
        // 1 : PERMISSION_DENIED
        // 2 : POSITION_UNAVAILABLE
        // 3 : TIMEOUT
        return defer.reject(err);
      }, options);

      return defer.promise;
    };
  });

