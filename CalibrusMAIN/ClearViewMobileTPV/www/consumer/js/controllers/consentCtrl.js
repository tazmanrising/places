"use strict";

angular.module('clearviewtpv')
    .controller('consentCtrl', function ($scope, $stateParams, $location, verifyService, accountService, persistService, scriptService,sessionService) {  //accountService){ //($scope, $state, $ionicModal, $ionicPopup, $ionicHistory, mobiscrollService, enrollmentService, userService, formValidationService, userFactory) {
        var vm = this;
        
        console.log('enter consent');
        
        //vm.blah = {};
        //console.log($stateParams.id);
        //vm.blah.tom = "the best";
        //console.log('test', vm.blah);

        //factory persistance   F5  and it is lost
        vm.passedData = {};
        vm.passedData = persistService.get();

        //console.log('vm.passedData', vm.passedData);

        // localStorage  F5 and it is not lost
        vm.Storage = {};
        vm.Storage.mainid = sessionService.get('mainid');
        vm.Storage.hash = sessionService.get('hash');
        



        console.log('vm.Storage.mainid',  vm.Storage.mainid);


        var canvas;
        var canvasWidth;
        var ctx;

        function init() {
            canvas = document.getElementById('signature');
            if (canvas.getContext) {
                ctx = canvas.getContext("2d");

                window.addEventListener('resize', resizeCanvas, false);
                window.addEventListener('orientationchange', resizeCanvas, false);
                resizeCanvas();
            }
        }

        function resizeCanvas() {
            canvas.width = window.innerWidth - 40;
            //canvas.height = window.innerHeight;
        }

        init();




        //return calibrusclearviewRequestService.submitRequest(requestData).then(function (resData) {
        //enrollmentService.resetEnrollment();
        //return resData;

        //     var $signature = document.getElementById("signature"),
        //       $signatureContext = $signature.getContext("2d"),
        //       lastMousePoint = {x:0, y:0};
        //   $signatureContext.strokeStyle = "#000000";
        //   $signatureContext.lineWidth = 1;

        var canvas = document.getElementById('signature');
        var signaturePad = new SignaturePad(canvas);



        // vm.clearCanvas = function () {
        //     console.log('test');
        //     signaturePad.clear();
        // };

        // vm.saveCanvas = function () {
        //     console.log('in save');
        //     var sigImg = signaturePad.toDataURL();
        //     //$scope.signature = sigImg;
        //     console.log(sigImg);
        //     vm.passedData.signature = sigImg;

        // }

        vm.passedData.value = [];

        vm.Check = function () {
            vm.passedData.value = _.filter($scope.choices, function (c) {
                return c.checked;
            });
            
        }


        vm.Submit = function() {
            var sigImg = signaturePad.toDataURL();
            //console.log(sigImg);
            vm.Storage.signature = sigImg;
            //sessionService.set('sigimg', sigImg);
            
            vm.Storage.verified = 1;

            console.log('storage',vm.Storage);
            
            

            var promise = accountService.postAccount(vm.Storage);

            // UPDATE Main with verification/concern
            
             promise.then(function (response){
                 
                 console.log('post success', response);
                 //sessionService.set('status', response);

             }, function(err){
                 sessionService.set('status', err);
                 console.log('err', err);
             });



            $location.path("/thankyou");
        }

        var getScripts = function (state) {
            vm.scripts = {};
            var promise = scriptService.getScripts(state);
            promise.then(function (response) {
                vm.scripts = response.data;
                console.log('response getscripts', response.data);
            }, function (err) {
                console.log('err', err);
            });
        };

        getScripts('MD');

        function convertCanvasToImage(canvas) {
            var image = new Image();
            image.src = canvas.toDataURL();
            //return image;
        }


        //pull up account
        var findOrder = function () {
            // SERVICE
            // var promise = verifyService.verifyDetails($stateParams.id)   
            // FACTORY
            vm.account = {};
            var promise = accountService.getAccount($stateParams.id);
            promise.then(function (response) {
                vm.account = response.data;
                console.log('response', response.data);

            }, function (err) {
                console.log('err', err);
            });

        }

        //findOrder();




    });
