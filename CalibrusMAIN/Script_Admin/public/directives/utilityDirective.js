
//Numbers only 

angular.module('calibrus')
.directive('numbersOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attr, ngModelCtrl) {
            function fromUser(text) {
                if (text) {
                    var transformedInput = text.replace(/[^0-9]/g, '');

                    if (transformedInput !== text) {
                        ngModelCtrl.$setViewValue(transformedInput);
                        ngModelCtrl.$render();
                    }
                    return transformedInput;
                }
                return undefined;
            }            
            ngModelCtrl.$parsers.push(fromUser);
        }
    };
});




/*
    Intended use:
    <phone-number placeholder='prompt' model='someModel.phonenumber' />
    Where: 
      someModel.phonenumber: {String} value which to bind formatted or unformatted phone number

    prompt: {String} text to keep in placeholder when no numeric input entered
*/
  angular.module('calibrus').directive('phoneNumber',
  ['$filter',
  function ($filter) {
    function link(scope, element, attributes) {

      // scope.inputValue is the value of input element used in template
      scope.inputValue = scope.phoneNumberModel;

      scope.$watch('inputValue', function (value, oldValue) {

        value = String(value);
        var number = value.replace(/[^0-9]+/g, '');
        scope.inputValue = $filter('phoneNumber')(number, scope.allowExtension);
        scope.phoneNumberModel = scope.inputValue;
      });
    }

    return {
      link: link,
      restrict: 'E',
      replace: true,
      scope: {
        phoneNumberPlaceholder: '@placeholder',
        phoneNumberModel: '=model',
        allowExtension: '=extension'
      },
      template: '<input ng-model="inputValue" type="tel" placeholder="{{phoneNumberPlaceholder}}" />'
    };
  }
  ]
)
/* 
    Format phonenumber as: (aaa) ppp-nnnnxeeeee
    or as close as possible if phonenumber length is not 10
    does not allow country code or extensions > 5 characters long
*/
.filter('phoneNumber', 
  function() {
    return function(number, allowExtension) {
      /* 
      @param {Number | String} number - Number that will be formatted as telephone number
      Returns formatted number: (###) ###-#### x #####
      if number.length < 4: ###
      else if number.length < 7: (###) ###
      removes country codes
      */
      if (!number) {
        return '';
      }

      number = String(number);
      number = number.replace(/[^0-9]+/g, '');
      
      // Will return formattedNumber. 
      // If phonenumber isn't longer than an area code, just show number
      var formattedNumber = number;

      // if the first character is '1', strip it out 
      var c = (number[0] == '1') ? '1 ' : '';
      number = number[0] == '1' ? number.slice(1) : number;

      // (###) ###-#### as (areaCode) prefix-endxextension
      var areaCode = number.substring(0, 3);
      var prefix = number.substring(3, 6);
      var end = number.substring(6, 10);
      var extension = number.substring(10, 15);

      if (prefix) {
        //formattedNumber = (c + "(" + area + ") " + front);
        formattedNumber = ("(" + areaCode + ") " + prefix);
      }
      if (end) {
        formattedNumber += ("-" + end);
      }
      if (allowExtension && extension) {
        formattedNumber += ("x" + extension);
      }
      return formattedNumber;
    };
  }
);