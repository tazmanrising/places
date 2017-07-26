"use strict";

angular.module('clearviewtpv')
  .controller('AppCtrl', function ($ionicPopup, $state, $ionicSideMenuDelegate) {
    
    console.log('in')
    
    this.logout = function () {
      $ionicPopup.confirm({
        title: 'Confirm',
        template: 'Are you sure you want to logout?'
      }).then(function (res) {
        if (res) {
          $ionicSideMenuDelegate.toggleLeft(false);
          //userService.logout();
          //$state.go('logon');
        }
      });
    };
  });
