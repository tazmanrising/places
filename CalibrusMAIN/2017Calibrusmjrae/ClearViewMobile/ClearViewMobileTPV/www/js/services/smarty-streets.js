"use strict";

angular.module('calibrus')
  .constant('smartStreetsDomainName', 'smartystreets.com')
  .constant('smartStreetsZipcodeUrl', 'https://us-zipcode.api.smartystreets.com')
  .constant('smartStreetsAuthId', '30044805091098764')
  .provider('smartStreetsApiInterceptor', function () {
    this.$get = ['$q', 'smartStreetsDomainName', 'smartStreetsAuthId', function ($q, smartStreetsDomainName, smartStreetsAuthId) {
      return {
        request: function (req) {
          if (new RegExp(`${smartStreetsDomainName}`).test(req.url)) {
            if (!req.params) req.params = {};
            req.params['auth-id'] = smartStreetsAuthId;
          }
          return req;
        }
      };
    }];
  })
  .config(function ($httpProvider) {
    $httpProvider.interceptors.push('smartStreetsApiInterceptor');
  })
  .service('smartyStreetsService', function ($http, smartStreetsZipcodeUrl) {
    this.getZipcodeInfo = function (zipcode) {
      return $http.get(`${smartStreetsZipcodeUrl}/lookup`, {
        params: {
          zipcode: zipcode
        }
      }).then(function (response) {
        return response.data;
      });
    };
  });
