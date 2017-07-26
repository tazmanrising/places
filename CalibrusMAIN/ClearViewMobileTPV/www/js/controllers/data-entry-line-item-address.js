"use strict";

angular.module('calibrus')
  .controller('DataEntryLineItemAddressCtrl', function ($ionicHistory, $state, $timeout, enrollmentService) {
    var lineItem = enrollmentService.getLineItem($state.params.lineItemIndex);
    $timeout(function () {
      if (!lineItem.serviceLocation) {
        $ionicHistory.currentView($ionicHistory.backView());
        $state.go('^.address-gps', {location: 'replace'});
      }
    }, 1);
  });
