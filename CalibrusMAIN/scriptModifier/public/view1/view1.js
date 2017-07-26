'use strict';

angular.module('myApp.view1', ['ngRoute'])

.config(['$routeProvider', function($routeProvider) {
  $routeProvider.when('/view1', {
    templateUrl: 'view1/view1.html',
    controller: 'View1Ctrl',
    controllerAs: 'vm'
  });
}])

.controller('View1Ctrl', ['$http',function($http) {
  var vm = this;
  vm.currentDB ;
  $http.get('/scripts')
      .then(function(response){
      //  console.log(response);
        vm.databases = response.data ;
      })

  vm.getTables = function(){
      //  console.log('where are the tables for %s', vm.currentDB.name)
    $http.get('/scripts/' + vm.currentDB.name)
        .then(function(response){
         // console.log(response.data[0]);
          vm.tables = response.data[0] ;
        })
  }

  vm.getScripts = function(){
    $http.get('/scripts/' + vm.currentDB.name + '/' + vm.currentTable.mytable)
        .then(function(response){
        //   console.log(response.data[0]);
          vm.scripts  = response.data[0];
        })
  }

  vm.saveScript = function(script){
      script.db = vm.currentDB ;
      script.table = vm.currentTable.mytable
      $http.post('/scripts/save', script )
          .then(function(response){
             // console.log(response);
                if(response.status == 200){
                    alert('record saved')
                }else
                {
                    alert('record not saved')
                }
              $('#myModal').modal({show:false}) ;
          })
  }

  vm.edit = function(script){
      // do modal
      vm.script = script;
     $('#myModal').modal({show:true}) ;
  }

}]);