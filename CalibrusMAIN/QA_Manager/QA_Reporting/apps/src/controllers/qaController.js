(function () {
    "use strict";

    //var injectParams = ['$scope', '$location', 'config', 'authService'];
    var injectParams = ['$scope', '$location', 'qaService'];

    var mainController = function ($scope, $location,qaService) {
        var vm = this;


        var promise = qaService.getAllQAList()
            .then(function (result) {
                //res = result;
                //console.log(result);
                vm = result;
                $scope.companies = result;
                console.log(vm);
                //scope.content = result;
                //console.log(res);
            })
            .catch(function () {
                console.log('problem');
            });

            $scope.quantity = 5;


    }



    mainController.$inject = injectParams;

    angular.module('qaManager').controller('mainController', mainController);


}());

angular
    .module("qaManager")
    .filter('isoConvert', function () {

        return function (str) {
            //var str2 = new Date('2016-08-26T16:02:15.747').toLocaleString('en-US');
            //var s = new Date(str).toLocaleString('en-US');
            var s = new Date(str).toLocaleString('en-US').replace(/,/, '');
            return s;
        }

    });