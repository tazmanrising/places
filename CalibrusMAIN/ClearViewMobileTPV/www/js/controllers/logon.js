"use strict";

angular.module('calibrus')
  .controller('LogonCtrl', function ($ionicLoading, $ionicPopup, $state, userService, geolocationService, userFactory) {
    var vm = this;

    //console.log('vm LogonCtrl', vm);

    vm.credentials = {};

    vm.processLocation = function () {
      vm.location = {
        error: null,
        geoposition: null
      };

      geolocationService.getCurrentPosition().then(function (geoposition) {
        vm.location.geoposition = geoposition;
        console.log('logonjs vm', vm);
      }, function (err) {
        vm.location.error = err;
      });
    };

    vm.processLocation();

    // from lat,lng we get zipcode. From zipcode we get city, state, zip

    vm.clearviewLogon = function () {
      $ionicLoading.show();

      userService.clearviewLogon(vm.credentials)
        .then(function () {
          $state.go('app.home');
        }, function (errorList) {
          $ionicPopup.alert({
            title: 'On no!',
            template: Array.isArray(errorList) ? errorList.join() : "Something went wrong"
          });
        }).finally(function () {
          userService.track()

          // var promise = userFactory.getServiceLocation();
          // promise.then(function (response){
          //   console.log('response logonjs',response);
          //   if(response.data.length > 0){
          //     //console.log('> 1');
          //     console.log('logon calling',response.data);
          //   }else{
          //     $ionicPopup.alert({
          //       title: 'Location Found',
          //       template: "You cannot sell here."
          //       //templateUrl: 'templates/modals/serviceZip.html', 
               
          //     });

          //     // $ionicModal.fromTemplateUrl('templates/modals/signature.html', {
          //     //     scope: 'blah',
          //     //     animation: 'slide-in-up'
          //     // });



          //   }

            

          // });          

        $ionicLoading.hide();
      });
    };
  });
