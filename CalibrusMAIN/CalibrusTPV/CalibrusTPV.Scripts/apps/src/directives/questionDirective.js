(function () {

    //var injectParams = ['$scope', '$location', '$routeParams',
    //                  '$timeout', 'config', 'dataService', 'modalService'];

    //var QuestionEditController = function($scope,
    //   $location,
    //   $routeParams,
    //   $timeout,
    //   config,
    //   dataService,
    //   modalService) {



    var questionDirective = function (questionService) {
        return {
            //replace: true,
            restrict: "E",
           // scope: {
           //     carrier: '@'
            // },
            templateUrl: 'apps/src/templates/questionTemplate.html'
           // template: '<div ng-bind-html="content"></div>',
            //link: function (scope, element, attrs) {
            //    var res = "";
            //    var promise = airlineService.getAllAirlines(attrs.carrier)
            //        .then(function (result) {
            //            res = result;
            //            scope.content = result;

            //        })
            //        .catch(function () {
            //            console.log('problem');
            //        });

            //}
        };
    };







    //QuestionEditController.$inject = injectParams;

    angular.module('calibrus').directive('questionDirective', questionDirective);

}());