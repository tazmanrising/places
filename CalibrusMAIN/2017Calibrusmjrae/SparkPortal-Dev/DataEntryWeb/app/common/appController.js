(function () {
    'use strict';

    angular
        .module('app')
        .controller('appController', appController);

    appController.$inject = ['$location']; 

    function appController($location) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'appController';

        activate();

        function activate() { 
            vm.currentDate = new Date();
        }
    }
})();
