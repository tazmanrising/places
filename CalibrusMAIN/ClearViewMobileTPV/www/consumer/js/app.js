/**
 * Created by sward on 5/26/2017.
 */
"use strict";
angular.module('clearviewtpv', [
  'ionic',
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
  'ngSanitize'
])
  .run(function ($rootScope, $http) {
    $http.defaults.headers.common['Auth-Token'] = 'U1BBUktUT0tFTg==';

  })
  .config(function ($httpProvider) {
  $httpProvider.defaults.headers.common = {};
  $httpProvider.defaults.headers.post = {};
  $httpProvider.defaults.headers.put = {};
  $httpProvider.defaults.headers.patch = {};
  $httpProvider.defaults.headers.get = {};
})
  .config(function ($ionicConfigProvider) {
    $ionicConfigProvider.views.maxCache(0);
    $ionicConfigProvider.backButton.previousTitleText(false).text('Back');
    //$ionicConfigProvider.platform.android.scrolling.jsScrolling(false);
  });
