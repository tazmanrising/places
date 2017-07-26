(function () {
    "use strict";

    var injectParams = ['$location', 'questionService', 'toastr', '$stateParams'];

    var questionController = function ($location, questionService, toastr, $stateParams) {

        var vm = this;


        
        //console.log('stateparam',blah);
        //http://localhost:3700/#/questions/32423432

        vm.question = {};

        vm.question.E911BrightPatternLoadFileId = "";
        vm.question.ph = "";
        vm.question.callid = $stateParams.id;
        vm.question.agentid = $stateParams.agentid;
        vm.question.agent = $stateParams.agent;
        vm.question.language = $stateParams.language;

        

        console.log(vm.question);

        vm.question.starttime = new Date();

        vm.question.active = true;

        //var url = $env.apiUrl + $env.apiBase + 'Manifest/VerifyChain/';
        //$http.put(url, data).then(function (response) {

        var loadQuestions = function () {
          
                var promise = questionService.getAllQuestions();
                promise.then(function (response) {
                    vm.myData = response;
                    //console.log('questionCtrl promise data', vm.myData);
                });
   
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


        //loadQuestions();

        vm.savePhone = function (ph) {

            var promise = questionService.getCustomer(ph);//8132644852 //9096080613
            promise.then(function (response) {


                if (!$.trim(response)) {
                     toastr.error('Phone Number Not Found', 'Error');
                }
                else {
                   
                    toastr.info('Phone Number Found!', 'Information');
                    vm.customer = {};
                    vm.customer = response;

                    vm.question.E911BrightPatternLoadFileId = vm.customer[0].E911BrightPatternLoadFileId;
                    vm.question.ph = vm.customer[0].TN;


                    console.log('vm.customer', vm.customer);
                    console.log('vm.cusomer[0].Name', vm.customer[0].Name);
                }
                //console.log('questionCtrl promise data', vm.myData);
            }, function (err) {
                console.log('err', err);
            });

        };

        vm.update911 = function (disposition) {

            console.log('dispostion', disposition);
            console.log('id', vm.question.E911BrightPatternLoadFileId);
            console.log('ph', vm.question.ph);

            var dispositionVerbiage = "";
            var dispositionNumber = 0;

            switch (disposition) {
                case "Verified":
                    dispositionNumber = 1;
                    dispositionVerbiage = "Verified"
                    break;
                case "Incorrect":
                    dispositionNumber = 3;
                    dispositionVerbiage = "Incorrect Number"
                    break;
                case "Refused":
                    dispositionNumber = 4;
                    dispositionVerbiage = "Customer Refused"
                    break;
                case "Changed":
                    dispositionNumber = 4;
                    dispositionVerbiage = "Customer Changed Mind"
                    break;
                case "Language":
                    dispositionNumber = 4;
                    dispositionVerbiage = "Language Barrier"
                    break;
                case "Confused":
                    dispositionNumber = 4;
                    dispositionVerbiage = "Customer Confused"
                    break;
                case "Agree":
                    dispositionNumber = 4;
                    dispositionVerbiage = "Does Not Agree"
                    break;
                case "Older":
                    dispositionNumber = 2;
                    dispositionVerbiage = "Not 18 Years or Older"
                    break;
                case "Questions":
                    dispositionNumber = 4;
                    dispositionVerbiage = "Customer Had Questions"
                    break;
                default:
                    dispositionNumber = 4;
                    dispositionVerbiage = "Unknown"
            }

            console.log(dispositionVerbiage);


            vm.customerCall = {};
            vm.customerCall.Id = vm.question.E911BrightPatternLoadFileId;
            vm.customerCall.ph = vm.question.ph;
            vm.customerCall.Verbiage = dispositionVerbiage;
            vm.customerCall.Number = dispositionNumber;
            vm.customerCall.WavName = vm.question.callid;
            vm.customerCall.StartTime = vm.question.starttime;
            vm.customerCall.AgentId = vm.question.agentid;
            vm.customerCall.AgentName = vm.question.agent;

            //handle data for call on api 

            var promise = questionService.updateFrontier911(vm.customerCall);
            //var promise = questionService.updateFrontier911(vm.question.E911BrightPatternLoadFileId, vm.question.ph, dispositionVerbiage, dispositionNumber);
            promise.then(function (response) {
                console.log('response', response);
                toastr.success('Call Completed Successfully!');
            }, function (err) {
                 toastr.error('Something went wrong', 'Error');
                console.log('err', err);
            });

        };


        vm.popup = function () {
            console.log('modal');
            $('#myModal').modal({ show: true });
        }

        vm.questionAnswer = function (choice, val) {
            console.log('choice', choice);
            console.log('val', val);

            if (val == "no") {
                vm.popup();
            } else if (choice == "speakwith" && val == "yes") {
                vm.customer.speakwith = "yes";
            } else if (choice == "e911" && val == "yes") {
                vm.customer.e911 = "yes";
            } else if (choice == "e18" && val == "yes") {
                vm.customer.e18 = "yes";
            }

        }





    }


    questionController.$inject = injectParams;

    angular.module('calibrus').controller('questionController', questionController);

}());