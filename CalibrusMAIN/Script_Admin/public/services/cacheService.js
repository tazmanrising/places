(function () {
    "use strict";


    angular
        .module('calibrus')
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
            /*
                Here "json_key" is the string key for getting the value in the localStorage.
                temp is the ram copy at any moment of the local storage.
                You can init temp with get.
                update return the new value.
                value should be the new json (I mean the whole json you want to store).
            */
            var temp = {};

            return {
                set: function (key, value) {
                    //temp = value;
                    //return localStorage.setItem("json_key", JSON.stringify(temp));
                    return localStorage.setItem(key, JSON.stringify(value));
                },
                get: function (key) {
                    //temp = JSON.parse(localStorage.getItem("json_key"));
                    //return temp;
                    return JSON.parse(localStorage.getItem(key));
                },
                destroy: function (key) {
                    return localStorage.removeItem(key);
                },
                update: function (key, value) {
                    temp.key = value;
                    set(temp);
                    return temp;
                }
            };
        }]);

    //  return factory;
    // };






}());