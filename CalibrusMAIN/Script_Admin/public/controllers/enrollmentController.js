(function () {
    'use strict';

    var enrollmentController = function (CustomerService, $http, $compile, $scope, $location, libertyService, sessionService) {

        var vm = this;
        vm.dataentry = {};
        vm.dataentry.accountChoice = 'show';
        vm.Storage = {};

        vm.Storage.salesChannel = sessionService.get('salesChannel');
        vm.Storage.agentid = sessionService.get('agentid');
        vm.Storage.state = sessionService.get('state');
        //vm.blah = "adfafd";
        console.log('state:', vm.Storage.state);

        var payload = function () {

            vm.cars = [
                { model: "Ford Mustang", color: "red" },
                { model: "Fiat 500", color: "white" },
                { model: "Volvo XC90", color: "black" }
            ];

            vm.carsObj = {
                car01: "Ford",
                car02: "Fiat",
                car03: "Volvo"
            };


            vm.carsObj2 = {
                car01: { brand: "Ford", model: "Mustang", color: "red" },
                car02: { brand: "Fiat", model: "500", color: "white" },
                car03: { brand: "Volvo", model: "XC90", color: "black" }
            };


            // vm.dataentry.states = {
            //     NY: "NY",
            //     CT: "CT"
            // };

            vm.dataentry.utilities = {
                Electric: "Electric",
                Gas: "Gas",
                Dual: "Electric & Gas"
            };

            vm.dataentry.utilities = CustomerService.getUtilityTypes();

            vm.getUtilities = function(){
                
                vm.nextthing = [];


                //getMarketState
                var promise = CustomerService.getUtilities("NY");//vm.dataentry.utility);
               //libertyService.getLibertyMarketState();
                promise.then(function (response) {
                    vm.dataentry.utils = response;
                    //console.log(' vm.dataentry.states', vm.dataentry.states)
                    //$("#state").val('NY');

                }, function (err) {
                    console.log(err);
                });




            }
           

            //http://maps.googleapis.com/maps/api/geocode/json?address=85298

            var url = "";

            var promise;
            

            vm.countries = CustomerService.getCountry();

            vm.getCountryStates = function(){
                vm.sates = CustomerService.getCountryState(vm.Country);
                vm.cities =[];
            }
            // url = "http://maps.googleapis.com/maps/api/geocode/json?address=85298"
            // $http.get(url).then(function(result){
            //     //vm.dataentry.google = result.data.results[0];


            //    vm.dataentry.googlecity = result.data.results[0].address_components[1].short_name; //   (gilbert)
            //    vm.dataentry.googlestate = result.data.results[0].address_components[3].short_name;    //  az
            //     console.log(vm.dataentry);

            // }, function(err){
            //     console.log('err getting agent validation', err);
            // });


            //https://jsfiddle.net/annavester/Zd6uX/

            //getMarketState
            // promise = libertyService.getLibertyMarketState();
            // promise.then(function (response) {
            //     vm.dataentry.states = response;
            //     console.log(' vm.dataentry.states', vm.dataentry.states)
            //     //$("#state").val('NY');

            // }, function (err) {
            //     console.log(err);
            // });

             vm.dataentry.state = 'NY';
        };



        payload();

        //$("#state").val(vm.Storage.state);
      
        


        vm.stateChange = function () {
            $("#state option:contains(" + vm.Storage.state + ")").attr('selected', 'selected');
        };




        vm.accountType = function (val) {

            vm.dataentry.accountType = val;
            console.log('acccount type ', val);
            vm.dataentry.accountChoice = 'hide';
        }



    };


    angular.module('calibrus').controller('enrollmentController', enrollmentController)


}());