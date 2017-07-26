(function () {


    angular.module('calibrus').directive('test', function ($compile, ) {
        return {
            restrict: 'E',
            scope: { text: '@' },
            template: '<p ng-click="add()">{{text}}</p>',
            controller: function ($scope, $element) {
                $scope.add = function () {
                    var el = $compile("<test text='n'></test>")($scope);
                    $element.parent().append(el);
                };
            }
        };

    });


    angular.module('calibrus').directive('myDirective', function ($compile) {
        return {
            restrict: 'E',
            scope: {
                data: '@'
            },
            template: '<h1>Whats Up {{data}}</h1><button ng-click="getAgent()">click</button>',
            controller: function ($scope, $element) {
                $scope.getAgent = function () {
                    alert($scope.data);
                }
            }
        };
    });

    angular.module('calibrus').directive('comDir', function ($compile) {
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

    angular.module('calibrus').directive("dTextbox", function ($http, $compile, sessionService) {
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

    angular.module('calibrus').directive("dAgent", function (libertyService, sessionService, $compile) {
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


    angular.module('calibrus').directive("dAgentw", function () {
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

    angular.module('calibrus').directive("dConcern", function () {
        return {
            //replace:true,
            restrict: "E",
            //template: '<input type="text" value="">'
            //templateUrl: '/templates/libertyTemplate.html',
            templateUrl: '/templates/agentTemplate.html',
        }
    });

    angular.module('calibrus').directive("dTextboxOLD", function () {
        return {
            restrict: "E",
            //template: '<input type="text" value="">'
            //templateUrl: '/templates/libertyTemplate.html',
            templateUrl: '/templates/channelTemplate.html',
        }   //d-textbox
    });


    angular.module('calibrus').directive("profile", function (libertyService, sessionService, $compile) {
        return {
            template: '<ng-include src="getTemplateUrl2()"/>',
            //templateUrl: unfortunately has no access to $scope.user.type
            scope: {
                user: '=data'
            },
            restrict: 'E',
            link: function (scope, element, attrs) {

                console.log('scope in directive', scope);

                //if agent ,... need to move this all to a service 

                scope.template = function (val) {
                    console.log('page check fx', val);
                }

                scope.validateAgent = function (val) {
                    console.log('blah', val);
                    if (typeof (response) == 'undefined') {
                        //todo  toaster 
                        // keep button disabled
                    } else {


                        promise = libertyService.validateAgent(val.agentid);
                        promise.then(function (response) {
                            //vm.agent = response[0].FirstName + " " + response[0].LastName;
                            //sessionService.set('agentid', agent.agentid);
                            //console.log('vm.agent', vm.agent);
                            console.log('response agent', response);

                            if (typeof (response) == 'undefined') {

                            } else {
                                var fullName = response[0].FirstName + " " + response[0].LastName;
                                angular.element(document.getElementById('agentName').innerHTML = fullName);
                                $compile(document.getElementById('agentName'));
                            }






                        }, function (err) {
                            console.log('ValidateAgent', err);
                        });

                    }


                }
            },
            controller: function ($scope, $compile) {
                //function used on the ng-include to resolve the template
                $scope.getTemplateUrl2 = function () {
                    //basic handling. It could be delegated to different Services


                    //console.log('$scope', $scope.user);


                    //return '/templates/channelTemplate.html';

                    // if ($scope.user.type == "twitter")
                    //     return "twitter.tpl.html";
                    // if ($scope.user.type == "facebook")
                    //     return "facebook.tpl.html";


                    //angular.element(document.getElementById('d1')).append($compile("<d-textbox></d-textbox>")(scope));



                    if ($scope.user.type == "agentid") {
                        //$compile("<d-agent></d-agent")(scope);
                        return '/templates/agentTemplate.html';
                    } else {
                        return '/templates/channelTemplate.html';
                    }



                }
            }
        };
    });

    angular.module('calibrus').directive('member', function ($compile) {
        return {
            restrict: "E",
            replace: true,
            scope: {
                member: '='
            },
            template: "<li></li>",
            link: function (scope, element, attrs) {
                if (angular.isArray(scope.member.children)) {
                    element.append("<collection collection='member.children'></collection>");
                    $compile(element.contents())(scope)
                }
            }
        }
    })

    angular.module('calibrus').directive('collection', function () {
        return {
            restrict: "E",
            replace: true,
            scope: {
                collection: '='
            },
            //template: "<input type='text' value='adf'>"
            template: "<ul><member ng-repeat='member in collection' member='member'></member></ul>"
        }
    })




    angular.module('calibrus').directive('dynamic', function ($compile) {
        return {
            restrict: 'A',
            replace: true,
            link: function (scope, ele, attrs) {
                scope.$watch(attrs.dynamic, function (html) {

                    ele.html(html);
                    $compile(ele.contents())(scope);
                });
            }
        };
    });


    angular.module('calibrus').directive('blueNote', function () {
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




    var airlineCode = function () {
        return {
            replace: true,
            restrict: "E",
            scope: {
                carrier: '@'
            },
            //template: '<div ng-bind-html="content"></div>',
            template: "<div>content</div>",
            link: function (scope, element, attrs) {
                var res = "";

                console.log('attrs.carrier', attrs.carrier);
                scope.content = '<input type="text" value="">';
                // var promise = airlineService.getAllAirlines(attrs.carrier)
                //     .then(function (result) {
                //         res = result;
                //         scope.content = result;

                //     })
                //     .catch(function () {
                //         console.log('problem');
                //     });

            }
        };
    };

    angular.module('calibrus').directive('airlineCode', airlineCode)



    angular.module('calibrus').directive("libertyDirective3", function () {
        return {
            restrict: "E",
            template: '<input type="text" value="">'
        }
    });


    var libertyDirective2 = function (libertyService) {
        return {
            replace: true,
            restrict: "E",
            scope: {
                id: '@'
            },
            //   template: `
            // <span>I am a custom directive <a href ng-click="clickMe()">Click Me</a></span>
            // `,
            template: `
            <input type="text" value="libertyDirective">
            `,
            //template: '<div ng-bind-html="content"></div>',
            //templateUrl: '/templates/libertyTemplate.html',
            link: function (scope, element, attrs) {

                //var res = "";

                //console.log('attrs.tag',attrs.tag);
                //console.log('scope.content', scope.content);

                var promise = libertyService.getDirectives(attrs.id)
                    .then(function (result) {
                        //res = result;
                        console.log('attrs id result', result[0]);
                        scope.content = result[0].Id;
                    }, function (err) {
                        console.log('err', err);
                    });
                //scope.content = attrs.tag; //result;
            }
        }
    }
    angular.module('calibrus').directive('libertyDirective2', libertyDirective2);
}());

angular.module('calibrus').directive("libertyDirective", function () {
    return {
        restrict: "E",
        template: '<input type="text" value="">'
    }
});

angular.module('calibrus').directive("showHide", function () {
    return {
        restrict: "E",
        template: '<input type="text" value="">'
    }   //d-textbox
});




angular.module('calibrus').directive("dYesno", function () {
    return {
        restrict: "E",
        template: `<button class="btn btn-primary">Yes</button>&nbsp;&nbsp;
        <button class="btn btn-danger">No</button>`
    }   //d-textbox
});







//Directive that returns an element which adds buttons on click which show an alert on click
angular.module('calibrus').directive("addbuttonsbutton", function () {
    return {
        restrict: "E",
        template: "<button addbuttons>Click to add buttons</button>"
    }
});

//Directive for adding buttons on click that show an alert on click
angular.module('calibrus').directive("addbuttons", function ($compile) {


    return function (scope, element, attrs) {

        //element.html(attrs.addbuttons);
        $compile(element.contents())(scope);
        //angular.element(document.getElementById('space-for-buttons')).append($compile("<div><button class='btn btn-default' data-alert="+scope.count+">Show alert #"+scope.count+"</button></div>")(scope));
        //angular.element(document.getElementById('space2')).append($compile("<div><button class='btn btn-default' data-alert="+scope.count+">Show alert #"+scope.count+"</button></div>")(scope));
        console.log('in')
        //angular.element(document.getElementById('space-for-buttons')).append($compile("<d-textbox></d-textbox>")(scope));
        angular.element(document.getElementById('d1')).append($compile("<d-textbox></d-textbox>")(scope));

        // element.bind("click", function(){
        // 	scope.count++;
        // 	angular.element(document.getElementById('space-for-buttons')).append($compile("<div><button class='btn btn-default' data-alert="+scope.count+">Show alert #"+scope.count+"</button></div>")(scope));
        // });
    };
});

//Directive for showing an alert on click
angular.module('calibrus').directive("alert", function () {
    return function (scope, element, attrs) {
        element.bind("click", function () {
            console.log(attrs);
            alert("This is alert #" + attrs.alert);
        });
    };
});



angular.module('calibrus').directive('compileDirective', function ($compile) {
    return function (scope, element, attrs) {
        //console.log(attrs.compileDirective); //<d-textbox></d-textbox>
        console.log(attrs.compileDirective);
        element.html(attrs.compileDirective);
        //console.log(attrs.compileDirective)

        //console.log(element.contents());
        $compile(element.contents())(scope);
    };
});

angular.module('calibrus').directive('xmyDirective', function () {
    return {
        template: `
      <span>I am a custom directive <a href ng-click="clickMe()">Click Me</a></span>
    `,
        link: function (scope, el, attrs) {
            scope.clickMe = function () {
                alert("I am a bear");
            }
        }
    }
});







angular.module('calibrus').directive('compileHtml', function ($compile) {
    return function (scope, element, attrs) {
        element.html(attrs.compileHtml);
        $compile(element.contents())(scope);
    };
});

angular.module('calibrus').directive('zmyDirective', function () {
    return {
        template: `
      <span>I am a custom directive <a href ng-click="clickMe()">Click Me</a></span>
    `,
        link: function (scope, el, attrs) {
            scope.clickMe = function () {
                alert("I am a bear");
            }
        }
    }
});