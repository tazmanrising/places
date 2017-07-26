"use strict";

angular.module('calibrus')
  .controller('DataEntryCtrl', function ($scope, $state, $ionicModal, $ionicPopup, $ionicHistory, mobiscrollService, enrollmentService, userService, formValidationService, userFactory) {
    var vm = this;

    vm.autocompleteOptions = {
      componentRestrictions: {country: 'usa'}
    };


    vm.mobiscroll = {
      phone: mobiscrollService.phone,
      select: mobiscrollService.select
    };

    // ========== Authorized Party ==========
    vm.setAuthorizedParty = function (authorizedParty) {
      enrollmentService.setAuthorizedParty(authorizedParty);
      /////////
     var promise = userFactory.getServiceLocation();
          promise.then(function (response){
            //console.log('response logonjs',response);
            if(typeof response === "undefined"){
              console.log('undefined');
            }else if(response.data.length > 0){
              console.log('logon calling',response.data);
            }else{
              $ionicPopup.alert({
                title: 'Location Found',
                template: "Not Licensed in this zip code."
                //templateUrl: 'templates/modals/serviceZip.html',

              });

            }

          });
      /////////////////////
      $state.go('app.data-entry.line-item.address', {lineItemIndex: 0});

    };

    vm.authorizedParty = angular.copy(enrollmentService.getAuthorizedParty());

    vm.processAuthorizedParty = function (formCtrl) {
      formValidationService.validateForm(formCtrl).then(function () {
        vm.setAuthorizedParty(vm.authorizedParty);
      });
    };

    // ========== Summary and Signature ==========
    vm.submitEnrollment = function () {
      var enrollment = enrollmentService.enrollmentToCalibrusRequest();
      userService.submitEnrollment(enrollment).then(function (data) {
        $ionicPopup.alert({
          title: 'Thank You!',
          template: 'Enrollment was submitted successfully! The confirmation number is ' + data.mainId
        }).then(function () {
          $ionicHistory.nextViewOptions({
            disableBack: true
          });
          $ionicHistory.clearHistory();
          $state.go('app.home');
        });
      }, function (err) {
        if (err.message) {
          $ionicPopup.alert({
            title: 'An error occurred!',
            template: err & err.message || 'Something went wrong!'
          });
        }
      });
    };
  });
