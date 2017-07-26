"use strict";

angular.module('clearviewtpv')
  .controller('consumerCtrl', function($scope, $location, $stateParams, verifyService, accountService, persistService,sessionService){  //accountService){ //($scope, $state, $ionicModal, $ionicPopup, $ionicHistory, mobiscrollService, enrollmentService, userService, formValidationService, userFactory) {
    var vm = this;
    console.log($stateParams.id);

    vm.newEnrollment = function(){
      $location.path("/consent");
    };

    //return calibrusclearviewRequestService.submitRequest(requestData).then(function (resData) {
    //enrollmentService.resetEnrollment();
    //return resData;

    //pull up account
    var findOrder = function() {

      // SERVICE
      // var promise = verifyService.verifyDetails($stateParams.id)   
      // FACTORY
      vm.account = {};
      vm.passedData = {};

      var promise = accountService.getAccount($stateParams.id);
      promise.then(function (response){
        
        
        vm.account = response.data;
        console.log('vm.account', vm.account);

        vm.passedData.hash = $stateParams.id;
        vm.passedData.mainid = vm.account[0].mainId;
        console.log(' vm.passedData.mainid', vm.passedData.mainid);
        persistService.set(vm.passedData); //change to subset

        console.log('response', response.data);

        sessionService.set('mainid', vm.account[0].mainId);
        sessionService.set('hash', $stateParams.id);
        


      }, function (err){
        console.log('err', err);
      });


    }

    findOrder();




  });
