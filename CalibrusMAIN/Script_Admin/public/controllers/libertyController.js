/* An IIFE removes variables from the global scope. 
This helps prevent variables and function declarations from living longer
than expected in the global scope, which also helps avoid variable collisions

ALSO:   When your code is minified and bundled into a single file 
for deployment to a production server, you could have collisions 
of variables and many global variables. 
An IIFE protects you against both of these by providing variable scope for each file

*/
(function () {
    'use strict';

    var libertyController = function ($compile, $scope, $location, libertyService, sessionService) {



        var vm = this;
        vm.liberty = {};
        vm.liberty2 = [];
        var promise = "";
        var promise2 = "";

        vm.answers = {};
        vm.Storage = {};


        sessionService.set('salesChannel', 7);





        $scope.host = 'jimmy';
        var fred = [];
        $scope.fred = {
            name: 'fred',
            body: 'blah',
            features: 'more stuff'
        }
        //fred.push('<my-directive data={{fred.name}}> duh</my-directive>')
        //fred.push('<my-directive data={{host}}> monkey </my-directive>')
        //fred.push('<my-directive data={{host}}> brains </my-directive>')
        //fred.push('<d-agent data={{host}}> </d-agent>')
        fred.push('<d-agent data="{{fred}}"> </d-agent>')
        fred.forEach(function (x) {
            //console.log(x)
            //   $("#directives").append($compile(x)($scope));
        });


        $scope.users = [
            // { name: "John", type: "twitter" },
            // { name: "Maria", type: "facebook" },
            { name: "agent", type: "agentid" }
        ];
        var allusers = [];
        allusers.push('<d-agent data="{{users}}"> </d-agent>')
        allusers.forEach(function (yy) {
            //    $("#directives").append($compile(yy)($scope));
        })


        console.log('lib 2', vm.liberty2);

        //Array.ForEach is about 95% slower than for() in for each for Arrays in JavaScript.
        // vm.liberty2.forEach(function (qq) {

        //     console.log('tom')
        //     console.log('directives',qq.directive);

        // })

        console.log(vm.liberty2.length);


        for (var i = 0, len = vm.liberty2.length; i < len; i++) {

            console.log(vm.liberty2[i]);

        }


        // cacheService.set('nav', vm.q);

        if (sessionService.get('nav') > 0) {
            vm.Storage.navigation = sessionService.get('nav');
            vm.q = vm.Storage.navigation;
            console.log('check session service', vm.q);
        } else {


            vm.q = 0;
            console.log('cannot find existing nav session setting to 1 : ', vm.q);
        }


        //Always track session
        console.log('page refresh the sessionService is:', sessionService.get('nav'));





        //console.log('xx', xx);

        // if (typeof vm.Storage.navigation != 'undefined'){
        //     vm.Storage.navigation = sessionService.get('nav');
        // }

        console.log('vm.Storage.navigation', vm.Storage.navigation);
        //vm.Storage.navigation = sessionService.get('nav');
        //console.log('aft vm.Storage.navigation',vm.Storage.navigation);

        vm.showdhide = "<show-hide>";

        $scope.count = 0;

        vm.data = [{
            "id": 1,
            "html": "adfaf",
            "directives": [{
                "abc": "<my-directive></my-directive>",
                "def": "other"
            },
            {
                "abc": "rrr",
                "def": "ggg"
            }
            ]

        }];
        //console.log('vm.data', vm.data);

        vm.test = '<a ng-click="click(1)" href="#">Click me</a>';

        var loadLiberty = function () {
            promise = libertyService.getLibertyQuestions();
            promise.then(function (response) {
                vm.liberty = response;

                vm.liberty.forEach(function (obj) {
                    //console.log('obj', obj);
                    promise2 = libertyService.getDirectives(obj.QuestionId)
                        .then(function (result) {
                            //console.log(result[0].Tag);
                            obj.directive = result;
                            vm.liberty2.push(obj);
                            //console.log('len', vm.liberty2.length);
                        }, function (err) {
                            console.log('err', err);
                        });

                });
                //console.log('len', vm.liberty2.length);
                console.log('vm.liberty', vm.liberty);
                //console.log('vm test', vm.liberty[0])         

            }, function (err) {
                console.log('err loadLiberty', err);
            });

            //////

        }

        loadLiberty();

        // vm.ValidateAgent = function (agent) {

        //     console.log('agent', agent.agentid);

        //     promise = libertyService.validateAgent(agent.agentid)
        //     promise.then(function (response) {
        //         vm.agent = response[0].FirstName + " " + response[0].LastName;
        //         sessionService.set('agentid', agent.agentid);
        //         console.log('vm.agent', vm.agent);

        //     }, function (err) {
        //         console.log('ValidateAgent', err);
        //     });

        // }

        vm.enrollment = function () {
            $location.path("/enrollment");
        };



        vm.getDirective = function (val) {
            
            //allusers.push('<d-agent data="{{users}}"> </d-agent>')

            //allusers.forEach(function (yy) {
            //$("#directives").append($compile(yy)($scope));
            //}
            
                console.log('get fx',val);


        };


        vm.getLocalStorage = function () {

            for (var i = 0, len = localStorage.length; i < len; ++i) {
                console.log('local Storage' + i + '=', localStorage.getItem(localStorage.key(i)));
            }
        }



        vm.questionContainer = function (question) {

            vm.q = question;
            console.log('questionContainer vm.q', vm.q);
            sessionService.set('nav', vm.q);
        }



        vm.navigation = function (direction) {


            if (direction === 'f') {

                if (vm.q == 0) {
                    console.log('vm.answers.channel', vm.answers.channel);
                    sessionService.set('channel', vm.answers.channel);
                }

                vm.q++;


            } else {
                if (vm.q > 0)  //todo  :   know upper limit of max questions
                    vm.q--;
            }

            console.log('directive 0', vm.liberty2[vm.q].directive[0].Tag);

            console.log('d', vm.liberty2[vm.q].directive[0].Tag);


            //angular.element(document.getElementById('qa').innerHTML = vm.liberty2[vm.q].directive[0].Tag);
            //$compile(document.getElementById('qa'));

            console.log('vm.q', vm.q);
            sessionService.set('nav', vm.q);
        }


        vm.init = function () {
            console.log('dd')
            console.log('init vm lib', vm.liberty);
            //vm.libdata = angular.copy(vm.liberty);
            console.log(vm.libdata);


            // vm.libdata.forEach(function (obj) {
            //     //console.log('obj', obj);
            //     promise2 = libertyService.getDirectives(obj.QuestionId)
            //         .then(function (result) {
            //             //console.log(result[0].Tag);
            //             obj.xtive = result[0].Tag;
            //         }, function (err) {
            //             console.log('err', err);
            //         });
            // });


        }





    };


    angular.module('calibrus').controller('libertyController', libertyController)


}());


angular.module('calibrus').filter('rawHtml', ['$sce', function ($sce) {
    return function (val) {
        console.log(val);
        return $sce.trustAsHtml(val);
    };
}]);