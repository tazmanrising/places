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
    var QuestionEditController = function ($location, questionService, $env) {

        var vm = this;

        vm.employee = {};

        console.log('pre employee', vm.employee);

        vm.submitForm = function () {

            console.log('employee', vm.employee);
            
        };

        
    };

    //QuestionEditController.$inject = injectParams;

    angular.module('calibrus').controller('QuestionEditController', QuestionEditController);

}());