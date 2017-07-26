(function () {

    angular.module('calibrus').directive('xcomDir', function ($compile) {
        return {
            restrict: "A",
            template: '<div>{{ blah }}</div>', // where myDirective binds to scope.myDirective
            scope: {
                comDir: '='
            },
            link: function (scope, element, attrs) {
                console.log('Do action with data', scope.comDir);
                console.log('d c', scope.comDir.Tag);

                //$compile(element.contents())(scope)

                scope.blah = scope.comDir; //$compile(scope.comDir.Tag)

                console.log('comdir scope', scope.comDir);
                console.log('scope.comDir.data', scope.comDir.data);

                //$("#directives").append($compile(scope.comDir.Tag)(scope));

                console.log('scope.comDir.Tag', scope.comDir.Tag);


                var data = scope.comDir.data;
                var tag = scope.comDir.Tag;
                var res = tag.replace("data=''", "data='" + data + "'");


                $("#directives").html($compile(res)(scope));
                //$("#directives").html($compile(scope.comDir.Tag)(scope));

            },

        }

    });



    angular.module('calibrus').directive("xdAgent", function (libertyService, sessionService, $compile) {
        return {
            //replace:true,
            //transclude: true,
            template: '<ng-include src="getTemplateUrl()"/>',
            restrict: "AE",
            scope: {
                data: '@'
                // user: '=data'
            },
            //template: '<input type="text" value="">'
            //templateUrl: '/templates/libertyTemplate.html',
            //templateUrl: '/templates/agentTemplate.html',
            link: function (scope, element, attrs) {
                console.log('scope agent', scope.data);
                // scope.validateAgent = function (val) {
                //     console.log('agent val', val);
                //     angular.element(document.getElementById('agentName').innerHTML = val); //"tom");
                //     //$compile(document.getElementById('agentName'));
                // }
                scope.validateAgent = function (val) {
                    console.log('blah', val);
                    // if (typeof (response) == 'undefined') {
                    //     //todo  toaster 
                    //     // keep button disabled
                    //     console.log('blah undefined');
                    // } else {


                    promise = libertyService.validateAgent(val.agentid);
                    promise.then(function (response) {
                        //vm.agent = response[0].FirstName + " " + response[0].LastName;
                        //sessionService.set('agentid', agent.agentid);
                        //console.log('vm.agent', vm.agent);
                        console.log('response agent', response);

                        if (typeof (response) == 'undefined') {
                            console.log('val agent undefined response');
                        } else {
                            sessionService.set('agentid', val.agentid);
                            var fullName = response[0].FirstName + " " + response[0].LastName;
                            angular.element(document.getElementById('agentName').innerHTML = fullName);
                            $compile(document.getElementById('agentName'));
                        }

                    }, function (err) {
                        console.log('ValidateAgent', err);
                    });

                    // }


                }

            },
            controller: function ($scope, $element) {
                console.log('in')
                $scope.getTemplateUrl = function () {

                    //if ($scope.user.type == "agentid") { ...}
                    //console.log($scope)

                    return '/templates/agentTemplate.html';
                }
            }
        }
    });

    angular.module('calibrus').directive("xdTextbox", function ($http, $compile, sessionService) {
        return {
            restrict: "AE",
            //templateUrl: '/templates/channelTemplate.html',
            template: '<ng-include src="getTemplateUrl()"/>',
            scope: {
                data: '@'
                // user: '=data'
            },
            link: function (scope, element, attrs) {
                console.log('scope textbox', scope.data);
                scope.validateZipCode = function (val) {
                    url = "http://maps.googleapis.com/maps/api/geocode/json?address=" + val
                    $http.get(url).then(function (result) {
                        //vm.dataentry.google = result.data.results[0];

                        console.log('zip result', result.data.results[0]);

                        var formatted_address = result.data.results[0].formatted_address; //"Lake Villa, IL 60046, USA";
                        var res = formatted_address.match(/^([^,]+),\s*([A-Z]{2})\b/);
                        if (res) {
                            console.log(res[1]);
                            console.log(res[2]);
                            var googlecity = res[1];
                            var googlestate = res[2];
                            sessionService.set('zip', val);
                            sessionService.set('city', googlecity);
                            sessionService.set('state', googlestate);
                        }



                        //var googlecity = result.data.results[0].address_components[1].short_name; //   (gilbert)
                        //var googlestate = result.data.results[0].address_components[3].short_name;    //  az
                        //console.log(vm.dataentry);
                        //sessionService.set('agentid', val.agentid);
                        var cityState = googlecity + ", " + googlestate;
                        angular.element(document.getElementById('CityState').innerHTML = cityState);
                        $compile(document.getElementById('CityState'));


                    }, function (err) {
                        console.log('err getting agent validation', err);
                    });

                }
            },
            controller: function ($scope, $element) {
                console.log('in')
                $scope.getTemplateUrl = function () {

                    //if ($scope.user.type == "agentid") { ...}
                    //console.log($scope)

                    return '/templates/textboxTemplate.html';
                }
            }
        }
    });




    angular.module('calibrus').directive("xdAgentw", function () {
        return {
            template: '<ng-include src="getTemplateUrl()"/>',
            restrict: "E",
            scope: {
                data: '@' // @ = isolate scope ( one-way bind)    & = pass isolate scope to parent scope
                //user: '=data'    //  =  is two-way binding between directives isolate scope and parent scope
                //
                //fred : '=data' 
                // users: '=data'
                //user:'=data'

            },
            // template: '<input type="text" value="">',
            //templateUrl: '/templates/agentTemplate.html',

            link: function (scope, element, attrs) {
                console.log('scope agent', scope.data);
                scope.validateAgent = function (val) {
                    console.log('agent val', val);
                    angular.element(document.getElementById('agentName').innerHTML = "tom");
                    //$compile(document.getElementById('agentName'));
                }
            },
            controller: function ($scope) {
                console.log('what');
                $scope.getTemplateUrl = function () {

                    console.log('bladfaf');

                    //         //if ($scope.user.type == "agentid") { ...}
                    return '/templates/agentTemplate.html';
                }
            }
        }
    });

    angular.module('calibrus').directive("xdConcern", function () {
        return {
            //replace:true,
            restrict: "E",
            //template: '<input type="text" value="">'
            //templateUrl: '/templates/libertyTemplate.html',
            templateUrl: '/templates/agentTemplate.html',
        }
    });

     angular.module('calibrus').directive('xblueNote', function () {
        return {
            replace: true,
            restrict: "E",
            scope: {
                text: '@'
            },
            template: '<div style="color:blue" ng-bind-html="content"></div>',
            link: function (scope, element, attrs) {
                scope.content = attrs.text;

            }
        }
    });

    angular.module('calibrus').directive("xdYesno", function () {
    return {
        restrict: "E",
        template: `<button class="btn btn-primary">Yes</button>&nbsp;&nbsp;
        <button class="btn btn-danger">No</button>`
    }   //d-textbox
});

}());