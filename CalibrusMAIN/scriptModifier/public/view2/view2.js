'use strict';

angular.module('myApp.view2', ['ngRoute'])

.config(['$routeProvider', function($routeProvider) {
  $routeProvider.when('/view2', {
    templateUrl: 'view2/view2.html',
    controller: 'View2Ctrl',
    controllerAs:'vm'
  });
}])

.controller('View2Ctrl', ['$http',function($http) {
var vm = this;
  $http.get('/scripts')
      .then(function(response){
        vm.databases = response.data ;
      })

  vm.getTables = function(){
    // console.log(vm.currentDB)
    $http.get('/main/' + vm.currentDB.name)
        .then(function(response){
          // console.log(response.data[0]);
          vm.tables = response.data[0] ;
        })
  }

  vm.getMain = function(){
    $http.get('/main/' + vm.currentDB.name + '/' + vm.currentTable.mytable)
        .then(function(response){
          // console.log(response.data[0]);
          vm.main  = response.data[0];
        })
  }

    vm.saveMain = function(main){
        main.db = vm.currentDB ;
        main.table = vm.currentTable.mytable
        $http.post('/main/save', main)
            .then(function(response){
                console.log(response);

            })
    }
  vm.edit = function(main){
    // do modal
    vm.EditMain = main;
    console.log('vm.editmain', vm.EditMain);
    $('#myModal').modal({show:true}) ;
  }

  vm.getmainid = function(){
    //  console.log('what are you searching for');
      $http.get('/main/'  + vm.currentDB.name + '/' + vm.currentTable.mytable + '/' + vm.mainid)
          .then(function(response){
            //  console.log('looking for a response')
              vm.main = [response.data[0]];
          })
  }

}]);