/**
 * Created by stephenward on 9/14/16.
 */
angular.module('myApp')
    .factory('callData',function($q,$http){


        return{
            insertCall: function(call){
                var defer = $q.defer();
                $http.post('/api/call',call)
                    .success(function(data){
                        defer.resolve(data)
                    })
                    .error(function(data){
                        defer.reject(data)
                    });
                return defer.promise;
            },
            getCall: function(callid){
                var defer = $q.defer();
                $http.get('/api/call' + callid)
                    .success(function(data){
                        defer.resolve(data)
                    })
                    .error(function(data){
                        defer.reject(data)
                    });
                return defer.promise;
            },
            updateCall: function(call){
                var defer = $q.defer();
                $http.post('/api/call/update',call)
                    .success(function(data){
                        defer.resolve(data)
                    })
                    .error(function(data){
                        defer.reject(data)
                    });
                return defer.promise;
            }
        }

    });