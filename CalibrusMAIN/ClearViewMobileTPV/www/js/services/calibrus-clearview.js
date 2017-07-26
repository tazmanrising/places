"use strict";

angular.module('calibrus')
  //FOR IONIC
 // .constant('calibrusclearviewUrl','')
  // NODE / Server
  //.constant('calibrusclearviewUrl', 'http://localhost:13497')
  .constant('calibrusclearviewUrl', 'https://clearview.calibrus.com')
  .provider('calibrusclearviewApiInterceptor', function () {
    this.$get = ['$q', function ($q) {
      return {
        response: function (res) {
          if (res.config.calibrusApiResponse) {
            var unknownError = ['No data was returned on response'];
            if (res.data.hasErrors) return $q.reject(res.data.errorList || unknownError);

            var data = res.data && res.data.data;
            if (!data) return $q.reject(unknownError);

            return data;
          }
          return res;
        }
      };
    }];
  })
  .config(function ($httpProvider) {
    $httpProvider.interceptors.push('calibrusclearviewApiInterceptor');
  })

  .service('calibrusclearviewLogonService', function ($q, $http, calibrusclearviewUrl) {

    this.logon = function (credentials) {
      return $http.post(`${calibrusclearviewUrl}/api/dataentry/logon`, {
        clearviewId: credentials.clearviewId,
        password: credentials.password
      }, {calibrusApiResponse: true});
    };

  })

  .service('calibrusclearviewRequestService', function ($q, $http, calibrusclearviewUrl, $window) {
  
    this.submitRequest = function (data) {
      //Could use sessionStorage
      //var sessionAddress = $window.sessionStorage.getItem('newAddress');
      //data.orderDetails[0].address = sessionAddress;
      
      console.log('request data',data);
      return $http.post(`${calibrusclearviewUrl}/api/request`, data, {calibrusApiResponse: true});
    };

    this.getLead = function (id, vendorNumber) {
      return $http.get(`${calibrusclearviewUrl}/api/lead/${vendorNumber}/${id}`, {calibrusApiResponse: true});
    };

    this.track = function (data) {
      return $http.post(`${calibrusclearviewUrl}/api/dtdtrack`, data, {calibrusApiResponse: true});
    };

    this.getServiceableZip = function(data){
      //http://localhost:22995
      return $http.get(`${calibrusclearviewUrl}/api/getserviceablezip/${data}`).then(function(res){
      //return $http.get(`http://localhost:22995/api/getserviceablezip/${data}`).then(function(res){
      //return $http.get(`http://localhost:22995/api/getserviceablezip/60030`).then(function(res){
        console.log('res.data calibrus clearview',res.data);
        return res.data;
      });
    };


    
    // NOTE : getMain clone..?
    this.getMain = function (id) {
      return $http.get(`${calibrusclearviewUrl}/api/main/${id}`, {calibrusApiResponse: true});
    };

    this.getProgramsForVendorByUtiltiyType = function (utilityId, vendorId, utilityType) {
      return $http.get(`${calibrusclearviewUrl}/api/programs/${utilityId}/${vendorId}/${utilityType}`, {calibrusApiResponse: true});
    };

    this.getUtilityTypes = function () {
      return $q.when([
        {
          type: 'electric',
          name: 'Electric',
          utilityType: 'electric'
        },
        {
          type: 'gas',
          name: 'Gas',
          utilityType: 'gas'
        },
        {
          type: 'dualFuel',
          name: 'Dual Fuel',
          utilityType: 'gas'
        }
      ]);
    };

    this.getUtilitiesByStateCode = function (stateCode) {
      if (!stateCode || (stateCode && stateCode.length !== 2)) return $q.reject('Invalid state');
      return $http.get(`${calibrusclearviewUrl}/api/utilities/${stateCode}`, {calibrusApiResponse: true});
    };

    this.getUtilityPrograms = function (vendorId, officeId, stateCode, zip) {
      return $http.get(`${calibrusclearviewUrl}/api/getutilityprograms/${vendorId}/${officeId}/${stateCode}/${zip}`).then(function (res) {
        //console.log(res);
        return res.data;
      });
    };

    this.getRelationships = function () {
      return $http.get(`${calibrusclearviewUrl}/api/relationships`, {calibrusApiResponse: true});
    };
  });
