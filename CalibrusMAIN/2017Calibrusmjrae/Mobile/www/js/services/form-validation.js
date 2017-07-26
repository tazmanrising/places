"use strict";

angular.module('calibrus')
  .service('formValidationService', function ($q, $ionicPopup) {
    this.validateForm = function (formCtrl) {
      if (!formCtrl && formCtrl.$setDirty) return $q.reject();
      formCtrl.$setDirty();

      if (formCtrl.$invalid) {
        return $ionicPopup.alert({
          title: 'Validation',
          template: 'Please fill out all require fields.'
        }).then(function () {
          return $q.reject();
        });
      }
      return $q.when(true);
    };
  });
