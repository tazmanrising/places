'use strict';

angular.module('scriptApp.scriptreview', ['ngRoute'])

.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/scriptreview', {
        templateUrl: 'views/scriptreview/scriptreview.html',
        controller: 'scriptreviewCtrl',
        controllerAs: 'vm'
    });
}])

.controller('scriptreviewCtrl', ['$http', function ($http) {
    var vm = this;
    
    vm.getScript = function(){
        $http.get('/api/scriptlookups/' + vm.myTable.Script)
           .then(function (response) {
               vm.currentScript = response.data;
               //vm.currentScript.forEach(function (s) {
               //    vm.history(s);
               //})
           })
    }
    $http.get('/api/scriptlookups')
        .then(function (response) {
            vm.scripts = response.data;
        });

    vm.history = function (s) {
        console.log("looking for scriptId %d %s", s.ScriptId, vm.myTable.Script);
        $http.get('api/scriptlookups/history/' + vm.myTable.Script + '/' + s.ScriptId)
        .then(function (response) {
            // open history modal window
            vm.modalHistory(response.data)
        })
    }
    vm.modal = function (script) {
        // do modal
        vm.scriptDetail = {};
        vm.scriptedit = script;
        angular.copy(script, vm.scriptDetail);
        $('#myModal').modal({ show: true });
    }

    vm.modalHistory = function (history) {
        // do modal history

        vm.scriptHistory = history;
        $('#modalHistory').modal({show:true})
    }

    vm.email = function () {
       // console.log('you trying to email ?')
        var mailobject = {};
        
        for (var key in vm.scriptDetail) {
            if (vm.scriptDetail.hasOwnProperty(key)) {
                if (vm.scriptedit[key] != vm.scriptDetail[key]) {
                    // console.log(key);
                    mailobject[key] = vm.scriptDetail[f]
                    
                }
                
               
            }
        }

        //$http.post('api/scriptlookups/email', mailobject)
        //  .then(function (response) {
        //      // do something
        //  }

        //)
    }
  
}]);