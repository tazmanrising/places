(function () {
    "use strict";

    var injectParams = ['$location', 'questionService', 'directiveService', 'toastr'];

    var questionController = function ($location, questionService, directiveService,  toastr) {

        var vm = this;
        vm.question = {};

        // ## Initialize object for ng-model
        vm.question.active = true;
        vm.question.Name = "";
        vm.question.ParentId = null;   
        vm.question.ParentValue = null;
        vm.question.Description = "";
        vm.question.Verbiage = "";
        vm.question.VerbiageSpanish = "";


        //var url = $env.apiUrl + $env.apiBase + 'Manifest/VerifyChain/';
        //$http.put(url, data).then(function (response) {

        var loadQuestions = function () {

            // if ($env.jsonTest === 0) {
            var promise = questionService.getAllQuestions();
            promise.then(function (response) {
                vm.myData = response;
                //console.log('questionCtrl promise data', vm.myData);
            });
            //} else {
            // console.log('in debug');

            // vm.myData = [{
            //     "Id": 2,
            //     "Name": "VendorId",
            //     "Description": "Ask agent what their vendor id is ",
            //     "Verbiage": "What Is Your Vendor ID",
            //     "VerbiageSpanish": "need spanish",
            //     "Active": null
            // },
            // {
            //     "Id": 3,
            //     "Name": "AgentId",
            //     "Description": "Ask Agent for their ID",
            //     "Verbiage": "What Is Your Agent ID",
            //     "VerbiageSpanish": "spanish",
            //     "Active": null
            // },
            // {
            //     "Id": 4,
            //     "Name": "TomTest",
            //     "Description": "Testing this",
            //     "Verbiage": "My hands felt just like two balloons\r\n\r",
            //     "VerbiageSpanish": "Mis manos se sentían como dos globos",
            //     "Active": true
            // }
            // ];
            // }
        }

        vm.add = function () {
            console.log(vm.question);
            vm.question = {};
            vm.question.Active = true;
            vm.question.Name = "";
            vm.question.ParentId = null;   
            vm.question.ParentValue = null;
            vm.question.Description = "";
            vm.question.Verbiage = "";
            vm.question.VerbiageSpanish = "";
            commonGets('getDirectives', 'liberty');



            //commonGets('getDirectiveAssoc', 'liberty');



            //$scope.color_ids = [];
            //vm.question.directive = [];

            // angular.forEach(vm.question, function (item) {
            //vm.question.directive[1].selected = false;
            // $('input:checkbox').attr('checked',false);
            //});

            $('#myModal').modal({
                show: true
            });


        }


        vm.isSelectedDirective = function (directive){
            console.log('directive', directive);
            //return vm.question.directiveassoc.findIndex((item) => item.directiveassoc[0].DirectiveId === directive) > 0 ? true : false;
            
            //$('input[name=directiveId'+directive+']').attr('checked', true);
            //$('input[name=directiveId1]').attr('checked', true);

            //$("#directiveId1").prop("checked", true);

            //document.getElementById("directiveId1").checked = true;

            $("#directiveId1").attr("checked", true);

        }




        vm.checkedDirective = function (dirIdChecked) {
            console.log('dirIdChecked', dirIdChecked)
            if (document.getElementById(dirIdChecked).checked) {
                console.log('yes')
            } else {
                console.log('should remove')
                console.log('test',  vm.directiveassoc[dirIdChecked])
                //vm.directiveassoc[dirIdChecked].data = null;
                //vm.directiveassoc[dirIdChecked].SortOrder = null; 
                $('#SortOrder' + [dirIdChecked]).val('');
                $('#data' + dirIdChecked).val('');




                //vm.question.data[1] = '';
            }
        }



        vm.isDirChecked = function (val) {
            //console.log('val: ', val);
            //console.log('in isDir', vm.directiveassoc);
            //vm.question.directive = [];

            //console.log('in isDirChecked vm question',vm.question)
            //console.log('in isDirChecked vm directiveassoc',vm.directiveassoc)
            //console.log('in isDir about to loop', val);

            //console.log('vm.directiveassoc',vm.directiveassoc[val]);

            for (var d in vm.directiveassoc) {
                if (val == vm.directiveassoc[d].DirectiveId) {
                    $('#SortOrder' + [val]).val(vm.directiveassoc[d].SortOrder);
                    $('#data' + [val]).val(vm.directiveassoc[d].data);

                    //console.log('in isDirChecked vm directiveassoc: '+val + '  ',vm.directiveassoc[val]);   

                    return true;
                }
            }
            return false;
        }

        vm.edit = function (script) {
            // do modal
            console.log('script', script);
            vm.question = script;
            //vm.question.name = "tomtest";
            //vm.question.description = vm.question.Description;

            commonGets('getDirectives', 'liberty');
            commonGets('getDirectiveAssoc', 'liberty');

            $('#myModal').modal({
                show: true
            });
        }

        var commonGets = function (type, client) {
            var promise = "";


            if (type === "getDirectives") {
                promise = directiveService.getDirectives();
                promise.then(function (response) {
                    vm.directives = "";
                    vm.directives = response;
                }, function (err) {
                    console.log('')
                });

            } else if (type === "getDirectiveAssoc") {
                promise = directiveService.getDirectiveAssoc(vm.question.Id);
                //console.log('vm.question.Id', vm.question.Id)
                promise.then(function (response) {
                    console.log('getassoc', response);
                    // vm.directiveassoc = {};
                    // vm.directiveassoc = response;

                    vm.question.directiveassoc = {};
                    vm.question.directiveassoc = response;



                    console.log('vm.question', vm.question);
                    console.log(' vm.directiveassoc', vm.question.directiveassoc);
                    //console.log('response.DirectiveId',response.DirectiveId);

                }, function (err) {
                    console.log('getDirectAssoc', err);
                });

            }



        }

        vm.saveQuestion = function (script) {
            var promise = "";

            var debug = 0;
            //console.log('savequestion debug', script);

            console.log('vm.question', vm.question)

            if (debug === 0) {



                if (!script.Id) {


                    console.log('script', script);




                    if (debug === 0) {
                        promise = questionService.createQuestion(script);
                        promise.then(function (response) {

                            script.Id = response.recordset[0]["QuestionId"];
                            vm.myData.push(script);

                            toastr.success('Question:', 'Added New Question!');

                        }, function (err) {
                            console.log('err', err);
                        });
                    }

                } else {

                    console.log('update script', script);

                    promise = questionService.updateQuestion(script);

                    promise.then(function (response) {
                        toastr.success('Question:', 'Updated Question!');
                    }, function (err) {
                        console.log('err', err);
                    });
                }

            }

        }


        loadQuestions();

    }


    questionController.$inject = injectParams;

    angular.module('calibrus').controller('questionController', questionController);

}());