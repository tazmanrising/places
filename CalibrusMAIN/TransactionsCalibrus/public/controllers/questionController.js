(function () {
    "use strict";

    var injectParams = ['$location', 'questionService', '$env', 'toastr'];

    var questionController = function ($location, questionService, $env, toastr) {

        var vm = this;

        vm.question = {};
        vm.question.active = true;

        //var url = $env.apiUrl + $env.apiBase + 'Manifest/VerifyChain/';
        //$http.put(url, data).then(function (response) {

        var loadQuestions = function () {

            if ($env.jsonTest === 0) {
                var promise = questionService.getAllQuestions();
                promise.then(function (response) {
                    vm.myData = response;
                    //console.log('questionCtrl promise data', vm.myData);
                });
            } else {
                console.log('in debug');

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

        vm.add = function () {
            console.log(vm.question);
            vm.question = {};
            $('#myModal').modal({ show: true });
        }

        vm.edit = function (script) {
            // do modal
            console.log('script', script);
            vm.question = script;
            //vm.question.name = "tomtest";
            //vm.question.description = vm.question.Description;
            $('#myModal').modal({ show: true });
        }



        vm.saveQuestion = function (script) {
            var promise = "";
            if (!script.Id) {
                promise = questionService.createQuestion(script);
                promise.then(function (response) {
                    
                    script.Id = response.recordset[0]["QuestionId"];
                    vm.myData.push(script);

                    toastr.success('Question:', 'Added New Question!');
                    

                });
            } else {
                promise = questionService.updateQuestion(script);
                promise.then(function (response) {
                       toastr.success('Question:', 'Updated Question!');
                });
            }


        }


        loadQuestions();

    }


    questionController.$inject = injectParams;

    angular.module('calibrus').controller('questionController', questionController);

}());