"use strict";

angular.module('calibrus', [
  'ionic',
  'google.places',
  'signature',
  'angular-cache',
  'mobiscroll-datetime',
  'mobiscroll-select',
  'mobiscroll-image',
  'mobiscroll-calendar',
  'mobiscroll-select',
  'mobiscroll-number',
  'mobiscroll-numpad',
  'mobiscroll-menustrip',
  'liveaddress'
])
  .run(function ($rootScope, geolocationService, $http) {
    $http.defaults.headers.common['Auth-Token'] = 'U1BBUktUT0tFTg==';
    $rootScope.hasGeolocation = geolocationService.hasGeolocation();
  })
  .run(function ($rootScope, $state, userService) {
    // Route Security
    $rootScope.$on('$stateChangeStart', function (e, to) {
      if (to.data && to.data.requiresLogin) {
        var user = userService.getUser();

        if (!user) { // Note could check if the user session is expired.
          e.preventDefault();
          $state.go('logon');
        }
      }
    });
  })
  .config(function ($ionicConfigProvider) {
    $ionicConfigProvider.views.maxCache(0);
    $ionicConfigProvider.backButton.previousTitleText(false).text('Back');
  });
