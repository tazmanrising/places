"use strict";

angular.module('calibrus')
  .directive('formValidation', function ($parse) {
    return {
      restrict: 'A',
      link: function (scope, element, attrs) {
        var model = $parse(attrs.formValidation)(scope),
          parentModel = $parse(attrs.formValidation.replace(/\.[^.]*?$/,''))(scope);

        var states = {
          error: false,
          mute: false,
          success: false
        };

        var keys = Object.keys(model).filter(function (n) {
          return /^\$[^\$]/.test(n);
        });

        var flipClass = function (bool, stateName) {
          var state = states[stateName];
          var className = 'has-' + stateName.toLowerCase();

          if (bool && !state) {
            element.addClass(className);
            states[stateName] = !states[stateName];
          }

          if (!bool && state) {
            element.removeClass(className);
            states[stateName] = !states[stateName];
          }
        };

        scope.$watch(function () {
          flipClass(parentModel.$pristine && model.$pristine, 'mute'); // This was a nice idea, however it doesn't know when the parent form state.
          flipClass(!parentModel.$pristine && !model.$valid, 'error');
          flipClass(!parentModel.$pristine && model.$valid, 'success');
        });
      }
    }
  });
