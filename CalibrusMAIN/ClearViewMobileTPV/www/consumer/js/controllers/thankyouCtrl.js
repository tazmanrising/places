"use strict";

angular.module('clearviewtpv')
  .controller('thankyouCtrl', function($scope, $location, $stateParams, verifyService, accountService, persistService,sessionService){  //accountService){ //($scope, $state, $ionicModal, $ionicPopup, $ionicHistory, mobiscrollService, enrollmentService, userService, formValidationService, userFactory) {
    var vm = this;

    vm.thankyou = {};
    //vm.thankyou.mainid = sessionService.get('mainid');
    //vm.thankyou.status = sessionService.get('status');



    console.log(vm.thankyou);

    //sessionService.destroy('mainid');
    //sessionService.destroy('hash');
    //sessionService.destroy('sigimg');
    //sessionService.destroy('status');

});
   