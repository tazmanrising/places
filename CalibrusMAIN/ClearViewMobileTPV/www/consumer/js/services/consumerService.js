
'use strict';

//NOT USED  
var accountService = function ($http, $q) {

    var baseUrl = "http://localhost:13497/api/";
    var factory = {};

    factory.getAccount = function (hash) {

        var url = "";
        url = baseUrl + "GetTPV/" + hash;
        url = "http://localhost:13497/api/checkmainverified/125722";
        var x = 125722

        url = `${calibrusclearviewUrl}/api/checkmainverified/${x}`;


        // url = "http://10.100.40.206:3500/api/liberty/scriptquestion";
        // return $http.post(url,
        //     {
        //         active: true,
        //         states: "AR",
        //         questions: "2",
        //         scriptorder: "100",
        //         salesChannel: "7",
        //         qtypeid: "1"
        //     }).then(function(result){
        //         console.log('update question result', result);
        //         return result;
        //     });




        return $http.get(url).then(function (result) {
            console.log('result', result);
            return result.data;
        });


    };


    return factory;

};

//http://blog.ionic.io/handling-cors-issues-in-ionic/


angular
    .module('clearviewtpv')
    //SERVER 
    .constant('calibrusclearviewUrl', 'https://clearview.calibrus.com')
    //LOCALHOST NODE
    //.constant('calibrusclearviewUrl', 'http://localhost:13497')
    //LOCALHOST IONIC
    //.constant('calibrusclearviewUrl', '') 
    // Lost vmware NODE LINUX SERVER 
    //.constant('calibrusclearviewUrl', 'https://206.169.51.164') 
    .factory('persistService', function () {
        var savedData = {};
        function set(data) {
            savedData = data;
        }
        function get() {
            return savedData;
        }

        return {
            set: set,
            get: get
        }

    })
    .factory('sessionService', ['$http', function ($http) {
        return {
            set: function (key, value) {
                return localStorage.setItem(key, JSON.stringify(value));
            },
            get: function (key) {
                return JSON.parse(localStorage.getItem(key));
            },
            destroy: function (key) {
                return localStorage.removeItem(key);
            },
        };
    }])
    .service('verifyService', function ($q, $http, calibrusclearviewUrl) {
        this.verifyDetails = function (hash) {
            return $http.get(`${calibrusclearviewUrl}/api/GetTPV/${hash}`);
            //return $http.get(`${calibrusclearviewUrl}/api/checkmainverified/${x}`, {calibrusApiResponse: true});

        }

        // return calibrusclearviewRequestService.submitRequest(requestData).then(function (resData) {
        //enrollmentService.resetEnrollment();
        //return resData;


    })
    
    .factory('scriptService', function ($q, $http, calibrusclearviewUrl) {
        var factory = {};

        factory.getScripts = function (state) {

            var url = `${calibrusclearviewUrl}/api/GetScripts/${state}`;

            return $http.get(url).then(function (result) {
                console.log('result', result);
                return result;
            }, function (err) {
                console.log('err', err);
            });
        }

        factory.getSingleScript = function (state, scriptid) {
            var url = `${calibrusclearviewUrl}/api/GetScripts/${state}/${scriptid}`;

            return $http.get(url).then(function (result) {
                console.log('result', result);
                return result;
            }, function (err) {
                console.log('err', err);
            });

        }

        return factory;

    })
    .factory('accountService', function ($q, $http, calibrusclearviewUrl) {
        var factory = {};



        factory.getAccount = function (hash) {

            var url = `${calibrusclearviewUrl}/api/GetTPV/${hash}`;

            return $http.get(url).then(function (result) {
                console.log('result', result);
                return result;
            }, function (err) {
                console.log('err', err);
            });

        }

        factory.postAccount = function(accountData){

            var url =  `${calibrusclearviewUrl}/api/PostTPV`;
            
            var req = {
                method: 'POST',
                url: url,
                headers: {
                    'Content-Type': "application/json"
                },
                data: accountData
            }
            
            return $http(req).then(function(result){
                console.log('posttpv',result);
                return result;
            }, function(err){
                console.log('posttpv err', err);
                return err;
            });
            
            
            // return $http.post(url, accountData).then(function(result){
            //     console.log('post result', result);
            //     return result;
            // }, function(err){
            //     console.log('err', err);
            // });

        }



        return factory;

    })
    //.factory('accountService', accountService)
    .provider('calibrusclearviewApiInterceptor', function () {
        this.$get = ['$q', function ($q) {
            return {
                response: function (res) {
                    if (res.config.calibrusApiResponse) {
                        var unknownError = ['No data was returned on response'];
                        if (res.data.hasErrors) return $q.reject(res.data.errorList || unknownError);

                        var data = res.data && res.data.data;
                        if (!data) return $q.reject(unknownError);

                        return data;
                    }
                    return res;
                }
            };
        }];
    })
    .config(function ($httpProvider) {
        $httpProvider.interceptors.push('calibrusclearviewApiInterceptor');
    });



