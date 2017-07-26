"use strict";

angular.module('calibrus')
  .constant('ipInfoUrl', 'https://ipinfo.io')
  .constant('ipInfoToken', 'dcd6ba675c0e70')
  .provider('ipInfoApiInterceptor', function () {
    this.$get = ['$q', 'ipInfoUrl', 'ipInfoToken', function ($q, ipInfoUrl, ipInfoToken) {
      return {
        request: function (req) {
          if (new RegExp(`${ipInfoUrl}`).test(req.url)) {
            if (!req.params) req.params = {};
            req.params.token = ipInfoToken;
          }
          return req;
        }
      };
    }];
  })
  .config(function ($httpProvider) {
    $httpProvider.interceptors.push('ipInfoApiInterceptor');
  })
  .service('ipInfoService', function ($http, ipInfoUrl, usaStatesService) {
    this.getIpInfo = function () {
      return $http.get(`${ipInfoUrl}`).then(function (res) {
        if (res.data && res.data.region) res.data.state = usaStatesService.getCodeFromName(res.data.region);
        return res.data;
      });
    };
  });
