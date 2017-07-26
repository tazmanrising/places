(function () {
    "use strict";

    var injectParams = ['$location', 'questionService', '$env'];

    var questionController = function($location, questionService, $env) {

        var vm = this;

        //var url = $env.apiUrl + $env.apiBase + 'Manifest/VerifyChain/';
        //$http.put(url, data).then(function (response) {
        
        if ($env.jsonTest === 0) {



            var promise = questionService.getAllQuestions();

            promise.then(function (response) {
                vm.myData = response;
                console.log('questionCtrl promise data', vm.myData);
            });

        } else {


            vm.myData = [
                {
                    "Id": 2,
                    "Name": "VendorId",
                    "Description": "Ask agent what their vendor id is ",
                    "Verbiage": "What Is Your Vendor ID",
                    "VerbiageSpanish": "need spanish",
                    "Active": null
                },
                {
                    "Id": 3,
                    "Name": "AgentId",
                    "Description": "Ask Agent for their ID",
                    "Verbiage": "What Is Your Agent ID",
                    "VerbiageSpanish": "spanish",
                    "Active": null
                },
                {
                    "Id": 4,
                    "Name": "TomTest",
                    "Description": "Testing this",
                    "Verbiage": "My hands felt just like two balloons\r\n\r",
                    "VerbiageSpanish": "Mis manos se sentían como dos globos",
                    "Active": true
                }
            ];
        }

    }


    questionController.$inject = injectParams;

    angular.module('calibrus').controller('questionController', questionController);

}());