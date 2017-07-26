(function () {
    "use strict";
  
   // <input type="text" name="number" only-digits>

    angular.module('calibrus')
        .directive('onlyDigits', function () {
            return {
                require: 'ngModel',
                restrict: 'A',
                link: function (scope, element, attr, ctrl) {
                    function inputValue(val) {
                        if (val) {
                            var digits = val.replace(/[^0-9]/g, '');

                            if (digits !== val) {
                                ctrl.$setViewValue(digits);
                                ctrl.$render();
                            }
                            return parseInt(digits, 10);
                        }
                        return undefined;
                    }
                    ctrl.$parsers.push(inputValue);
                }
            };
        });
    // .directive('numbersOnly', function () {
    //     return {
    //         restrict: 'AE',
    //         replace: 'true',
    //         require: 'ngModel',
    //         link: function (scope, element, attrs) {

    //             modelCtrl.$parsers.push(function (inputValue) {
    //                 // this next if is necessary for when using ng-required on your input. 
    //                 // In such cases, when a letter is typed first, this parser will be called
    //                 // again, and the 2nd time, the value will be undefined
    //                 if (inputValue == undefined) return ''
    //                 var transformedInput = inputValue.replace(/[^0-9]/g, '');
    //                 if (transformedInput != inputValue) {
    //                     modelCtrl.$setViewValue(transformedInput);
    //                     modelCtrl.$render();
    //                 }

    //                 return transformedInput;
    //             });

    //         }


    //     }

    // });
}());

/*{<div ng-controller="MyCtrl">
    <input type="text" ng-model="number" required="required" numbers-only="numbers-only" />
</div>

angular.module('myApp', []).directive('numbersOnly', function () {
    return {
        require: 'ngModel',
        link: function (scope, element, attrs, modelCtrl) {
            modelCtrl.$parsers.push(function (inputValue) {
                // this next if is necessary for when using ng-required on your input. 
                // In such cases, when a letter is typed first, this parser will be called
                // again, and the 2nd time, the value will be undefined
                if (inputValue == undefined) return ''
                var transformedInput = inputValue.replace(/[^0-9]/g, '');
                if (transformedInput != inputValue) {
                    modelCtrl.$setViewValue(transformedInput);
                    modelCtrl.$render();
                }

                return transformedInput;
            });
        }
    };
});

function MyCtrl($scope) {
    $scope.number = ''
}}*/