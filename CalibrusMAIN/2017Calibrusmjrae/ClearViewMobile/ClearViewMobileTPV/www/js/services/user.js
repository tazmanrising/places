"use strict";

angular.module('calibrus')
  .service('userService', function ($q, userCache, userHttpCache, calibrusclearviewLogonService, calibrusclearviewRequestService, geolocationService, addressService, enrollmentService) {
    var _this = this;
    var cachedGeoposition = null;
    
    console.log("_this userService", _this);

    var storeUser = function (user) {
      userCache.put('user', user);
      return _this.getUser();
    };

    var storeGeoposition = function (geoposition) {
      /*
       // Version 1: This is creating a copy of the object which makes the data mutable. This is not guaranteed safe data.
       userCache.put('geoposition', {
       coords: {
       accuracy: geoposition.coords.accuracy,
       altitude: geoposition.coords.altitude,
       altitudeAccuracy: geoposition.coords.altitudeAccuracy,
       heading: geoposition.coords.heading,
       latitude: geoposition.coords.latitude,
       longitude: geoposition.coords.longitude,
       speed: geoposition.coords.speed
       },
       timestamp: geoposition.timestamp
       });
       return _this.getGeoposition();
       */

      // Version 2: Cache the actual object inside this service.
      cachedGeoposition = geoposition;
      return cachedGeoposition;
    };

    var storeIpInfo = function (ipInfo) {
      userCache.put('ipInfo', ipInfo);
    };

    var saveSession = function (user) {
      return $q.all([geolocationService.getCurrentPosition(), addressService.ipInfo()])
        .then(function (results) {
          return {
            user: storeUser(user),
            geoposition: storeGeoposition(results[0]),
            ipInfo: storeIpInfo(results[1])
          };
        });
    };

    this.getUser = function () {
      return userCache.get('user');
    };

    this.getGeoposition = function () {
      // Version 1: Return the mutable cached version
      // return userCache.get('geoposition');

      // Version 2: Return the reference to the actual object which is immutable
      if (cachedGeoposition && cachedGeoposition.timestamp) return $q.when(cachedGeoposition);
      return geolocationService.getCurrentPosition().then(function (geoposition) {
        return storeGeoposition(geoposition);
      });
    };

    this.getIpInfo = function () {
      return userCache.get('ipInfo');
    };

    this.clearviewLogon = function (credentials, geoposition) {
      _this.logout();

      return calibrusclearviewLogonService.logon(credentials)
        .then(function (data) {
          return saveSession(data, geoposition);
        });
    };

    this.submitEnrollment = function (enrollment) {
      var requestData = angular.extend(enrollment, {
        user: _this.getUser(),
        ipLocation: _this.getIpInfo()
      });
      return calibrusclearviewRequestService.submitRequest(requestData).then(function (resData) {
        enrollmentService.resetEnrollment();
        return resData;
      });
    };

    this.track = function(){
      try{
        var user = _this.getUser();
        var data = {};
        data.AgentId = user.agentId;
        data.Geolocation = {};
        data.Geolocation.lat = cachedGeoposition.coords.latitude;
        data.Geolocation.lng = cachedGeoposition.coords.longitude;
        // console.log(data);
        calibrusclearviewRequestService.track(data);
       } catch (err) {
        console.log(err);
      }
    };

    this.logout = function () {
      userHttpCache.removeAll();
      userCache.removeAll();
        enrollmentService.resetEnrollment();
    }
  });
