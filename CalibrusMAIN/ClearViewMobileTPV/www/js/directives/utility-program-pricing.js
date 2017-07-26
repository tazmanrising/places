"use strict";

angular.module('calibrus')
  .directive('utilityProgramPricing', function () {
    return {
      replace: true,
      templateUrl: 'js/directives/utility-program-pricing.html',
      scope: {
        utilityAndProgram: '=program'
      }
    }
  });
