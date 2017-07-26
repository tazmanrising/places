'use strict';

angular.module('scriptApp.ScriptViewer', ['ngRoute'])

    //Set up environment for the client - which page to use for angular    
.config(['$routeProvider', function ($routeProvider) {
    $routeProvider.when('/ScriptViewer', {
        templateUrl: 'Views/ScriptViewer/ScriptViewer.html',
        controller: 'ScriptViewerCtrl',
        controllerAs: 'vm'
    });
}])

.controller('ScriptViewerCtrl', ['$http', function ($http) {

    var vm = this;
    vm.ScriptViewerLoading = true;

    vm.getScript = function () {
        //console.log('GetScriptValue: %s', vm.ScriptViewer); 
        //Get the Specific Script for the gridview
        $http.get('/api/scriptlookups/' + vm.ScriptViewer.Script)
          .then(function (response) {
              vm.currentScript = response.data;
          })
    }

    //Gets the list for the dropdown
    $http.get('/api/scriptLookups')
        .then(function (response) {
            vm.scriptLookupList = response.data;
            //console.log(response)
            if (vm.scriptLookupList) {
                vm.ScriptViewerLoading = false;
            }
        });

    vm.getHistory = function (s) {
        console.log("looking for scriptId %d for %s", s.ScriptId, vm.ScriptViewer.Script);
        $http.get('api/scriptlookups/history/' + vm.ScriptViewer.Script + '/' + s.ScriptId)
        .then(function (response) {
            vm.modalHistory(response.data)
        })
    }

    vm.modalEmail = function (script) {

        vm.scriptChanges = script;//capture changed values for emailing
        vm.scriptDetail = {};
        angular.copy(script, vm.scriptDetail); //preserve the parent modal windows script values
        // do modal email     
        $('#modalEmail').modal({ show: true });
    }

    vm.modalHistory = function (history) {
        // do modal history
        vm.scriptHistory = history;
        $('#modalHistory').modal({ show: true });
    }

    vm.emailScriptChanges = function () {

        console.log('ScriptId to change s%', vm.scriptDetail.ScriptId);
        //console.log('email Script Changes method hit');
        var isChanged = false;//flag to see if we have a delta
        //Values to pass in no matter what changes are made on the ModalHistory phone.
        var mailScriptChanges = {
            Client: "Constellation",
            Script: vm.ScriptViewer.Script,
            ScriptId: vm.scriptDetail.ScriptId,
            ScriptOrder: vm.scriptDetail.ScriptOrder
        };

        //Loop through the vm.scriptDetail from the Modal window
        //to determine the delta on the values they want emailed 
        for (var key in vm.scriptDetail) {
            if (vm.scriptDetail.hasOwnProperty(key)) {

                //if there is a delta in the values coming back
                if (vm.scriptChanges[key] != vm.scriptDetail[key]) {
                    //console.log(key);
                    mailScriptChanges[key] = vm.scriptDetail[key]; //assign the values to mailScriptChanges  
                    if (mailScriptChanges[key] == "") {
                        mailScriptChanges[key] = "USER REQUESTS DELETE VALUE"; //If a user has changed the field to be deleted insert this message as the value
                    }
                    isChanged = true;
                }
            }
        }

        console.log("isChanged %s", isChanged);
        //If a script has been changed Post to endpoint
        if (isChanged) {
            $http.post('api/scriptLookups/emailChanges', mailScriptChanges)//post to endpoint with changes
            .then(function (response) {
                //console.log(response);
                //if (response.headers.status = 200) {
                //    console.log("You Win: %s", response.headers.status);
                //}
                //else {
                //    console.log("You Fail: %s", response.headers.status);
                //}
            });
        }
    }


}]);