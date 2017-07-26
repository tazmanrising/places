"use strict";

angular.module('calibrus')
  .controller('DataEntrySummaryAndSignatureCtrl', function ($scope, $ionicModal, $ionicPopup, enrollmentService) {
    var vm = this;

    vm.enableSignature = true;
    

    vm.refreshEnrollment = function () {
      vm.order = enrollmentService.getEnrollment();
      vm.hasSigned = !!vm.order.signature;
    };

    vm.signaturePad = {};
    vm.signature = null;

    $ionicModal.fromTemplateUrl('templates/modals/signature.html', {
      scope: $scope,
      animation: 'slide-in-up'
    }).then(function (modal) {
      vm.modal = modal;
    });

    vm.toggleAgreeAndSign = function () {
      if (!vm.order.signature) return vm.showSignatureModal();

      $ionicPopup.confirm({
        title: 'Confirm',
        template: 'Are you sure you want to remove your signature?'
      }).then(function (res) {
        if (res) {
          enrollmentService.setSignature(null);
        }
        vm.refreshEnrollment();
      });
    };

    vm.toggleContactPreference = function (contactPreference) {
      enrollmentService.setContactPreference(contactPreference);
      vm.refreshEnrollment();
    };

    vm.showSignatureModal = function () {
      vm.modal.show();
    };

    vm.sign = function (signature) {
      enrollmentService.setSignature(signature);
      vm.modal.hide();
    };

    $scope.$on('modal.hidden', function() {
      vm.refreshEnrollment();
    });

    $scope.$on('$destory', function () {
      vm.modal && vm.modal.remove && vm.modal.remove();
    });

    vm.refreshEnrollment();
  });
