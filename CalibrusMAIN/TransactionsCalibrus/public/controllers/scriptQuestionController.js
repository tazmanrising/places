(function () {
    "use strict";

    var injectParams = ['$location', 'scriptQuestionService', 'questionService', '$env', 'toastr'];

    var scriptQuestionController = function($location, scriptQuestionService, questionService, $env, toastr) {

        var vm = this;
        vm.scriptQuestion = {};
        vm.scriptQuestion.active = true;
        vm.scriptQuestions = {};


        var loadStates = function(){
            var promise = scriptQuestionService.getStates();
            promise.then(function(response){
                vm.states = response;
            });
        }
        loadStates();
        if(typeof vm.StateName ==="undefined") {
            console.log(vm.StateName);
            
            vm.StateName = "AA";
        }

        var loadSalesChannel = function(){
            var promise = scriptQuestionService.getSalesChannel();
            promise.then(function(response){
                vm.salesChannel = response;
            });
        }
        loadSalesChannel();

        //if(typeof vm.SalesChannelId ==="undefined") {
        //    vm.SalesChannelId = "7";  // 7 is DTD
        //}

        var getQuestions = function(){
            var promise = questionService.getAllQuestions();
            promise.then(function(response){
                vm.questions = response;
            });
        }

        getQuestions();

        var getScriptQuestions = function(StateCode,SalesChannelId){
            var promise = scriptQuestionService.getAllScriptQuestions(StateCode,SalesChannelId);
            promise.then(function(response){
                console.log('getscriptquestion',response);
                vm.scriptQuestions = response;
            });
        }

        getScriptQuestions('AA', '3');


        vm.selectScriptChange = function(){
   
            if(typeof vm.SalesChannelId != "undefined") {
                getScriptQuestions(vm.StateCode, vm.SalesChannelId);
            }

        }

        // add script question
        vm.addScript = function(){
            if(typeof vm.SalesChannelId != "undefined") {
                vm.scriptQuestion = {};
                vm.scriptQuestion.StateCode = vm.StateName;
                vm.scriptQuestion.SalesChannelId = vm.SalesChannelId;
                
                $('#scriptModal').modal({show:true});
            }else{
                 toastr.error('Please pick a Sales Channel', 'Error');
            }
        }
      
        //add question
        vm.add = function(){

            $('#myModal').modal({show:true}) ;
        }

        // edit script question
        vm.edit = function (script) {
            // do modal
            //console.log('edit script', script);
            //console.log('vm.scriptQuestion before', vm.scriptQuestion);
            //console.log('vm.questions', vm.questions);
            vm.scriptQuestion = script;

            console.log('script q id', script.QuestionId);
            vm.scriptQuestion.QuestionId = script.QuestionId.toString(); //"28";
            vm.scriptQuestion.QtypeId = script.QtypeId.toString();
            console.log('vm.scriptQuestion after', vm.scriptQuestion);
            //vm.scriptQuestion.ScriptOrder = 500;
            //vm.question.name = "tomtest";
            //vm.question.description = vm.question.Description;
            $('#scriptModal').modal({ show: true });
        }

      
        vm.saveQuestion = function (script) {
            var promise = "";
            if (!script.Id) {
                promise = questionService.createQuestion(script);
                promise.then(function (response) {
                    script.Id = response.recordset[0]["QuestionId"];
                    vm.questions.push(script);
                    toastr.success('Question:', 'Added New Question!');
                });
            } else {
                promise = questionService.updateQuestion(script);
                promise.then(function (response) {
                     toastr.success('Question:', 'Updated Question!');
                });
            }
        }

        vm.saveScriptQuestion = function(script){
            var promise = "";
            if(!script.ScriptId){
                promise = scriptQuestionService.createScriptQuestion(script);
            
                promise.then(function(response){
                    //console.log('promise saveScriptQuestion', response);
                    //loadQuestions();
                    //getQuestions();
                    console.log('response.recordset[0]',response.recordset[0]);
                    script.ScriptId = response.recordset[0]["ScriptId"];
                    script.QTypeName = response.recordset[0]["QTypeName"];                    
                    script.SalesChannel = response.recordset[0]["SalesChannel"]; 
                    script.StateName = response.recordset[0]["StateName"]; 
                    script.Verbiage = response.recordset[0]["Verbiage"]; 
                    vm.scriptQuestions.push(script);
                    toastr.success('Script Question:', 'Added New Script Question!');
                });
                

            }else{
                promise = scriptQuestionService.updateScriptQuestion(script);
                 console.log('save script q. res', script);
                promise.then(function (response) {
                   
                    toastr.success('Script Question:', 'Updated Script Question!');
                });   
            }
          
        }


        

    }


    scriptQuestionController.$inject = injectParams;

    angular.module('calibrus').controller('scriptQuestionController', scriptQuestionController);

}());