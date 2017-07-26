"use strict";

angular.module('calibrus')
  .constant('googleUrl', 'https://maps.googleapis.com')
  .constant('googleApiKey', 'AIzaSyCVWozJgXKF65-HdpAZ8y_hg-oHKc6-dJY')
  .provider('googleApiInterceptor', function () {
    this.$get = ['$q', 'googleUrl', 'googleApiKey', function ($q, googleUrl, googleApiKey) {
      return {
        request: function (req) {
          if (new RegExp(`^${googleUrl}`).test(req.url)) {
            if (!req.params) req.params = {};
            req.params.key = googleApiKey;
          }
          return req;
        }
      };
    }];
  })
  .config(function ($httpProvider) {
    $httpProvider.interceptors.push('googleApiInterceptor');
  })
  .service('googleService', function ($q, $http, googleUrl, googleApiKey) {
    this.googleAddressComponentsToAddress = function (location, customMapping) {
      var map = customMapping || {
          street_number: 'address1',
          route: 'address1',
          locality: 'city',
          administrative_area_level_1: 'state',
          postal_code: 'zip'
        };

      if (!location) return null;
      return location.address_components.reduce(function (obj, c) {
        for (var i = 0; i < c.types.length; i++) {
          var type = c.types[i];
          if (map[type]) {
            if (!obj[map[type]]) obj[map[type]] = '';
            else obj[map[type]] += ' ';
            obj[map[type]] += c.short_name; // NOTE : could use long or short.
            break;
          }
        }
        return obj;
      }, {
        verified: true,
        formattedAddress: location.formatted_address.replace(', USA', '')
      });
    };

    //TODO  - delete this after success of new getReverseGeocode with undefined header 
    
    // this.getReverseGeocode = function (coords) {
    //   return $http.get(`${googleUrl}/maps/api/geocode/json`, {
    //     params: {
    //       latlng: `${coords.latitude.toFixed(5)},${coords.longitude.toFixed(5)}`,
    //       locationType: 'ROOFTOP',
    //       result_type: 'street_address'
    //     }
    //   }).then(function (res) {
    //     if (res.data.status === "REQUEST_DENIED") return $q.reject(res.data.error_message);
    //     if (!res.data.results || !Array.isArray(res.data.results)) return $q.reject('Unable to reverse geocode.');
    //     return res.data;
    //   });
    // };

     this.getReverseGeocode = function (coords) {
       var req = {
            method: 'GET',
            url:  `${googleUrl}/maps/api/geocode/json`,
            headers: {
              'Auth-Token': undefined
            },
            params: {
                latlng: `${coords.latitude.toFixed(5)},${coords.longitude.toFixed(5)}`,
                locationType: 'ROOFTOP',
                result_type: 'street_address'
            }
        }
      
        return $http(req)
          .then(function(res){
              if (res.data.status === "REQUEST_DENIED") return $q.reject(res.data.error_message);
              if (!res.data.results || !Array.isArray(res.data.results)) return $q.reject('Unable to reverse geocode.');
              return res.data;
          });
    };

    this.getStaticMapUrl = function (latitude, longitude, settings) {
      settings = angular.extend({
        zoom: 14,
        size: {width: 400, height: 400}
      }, settings);

      // Note : Setting image to http as https won't load unless a key and specific conditions are met.
      var mapUrl = `https://maps.googleapis.com/maps/api/staticmap?center=${latitude},${longitude}&zoom=${settings.zoom}&size=${settings.size.width}x${settings.size.height}`;
      if (Array.isArray(settings.markers)) mapUrl += settings.markers.reduce(function (str, marker) {
        return str += `&markers=color:${marker.color}%7C${marker.latitude},${marker.longitude}`;
      }, '');


      return mapUrl += `&key=${googleApiKey}`;

    };

  });
