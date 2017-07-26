﻿(function () {
    'use strict';

    angular
        .module('app')
        .controller('requestController', requestController);

    requestController.$inject = ['$location', '$state', '$stateParams', '$log', '$cookies', '$window', '$filter', 'logonService', 'requestService'];

    function requestController($location, $state, $stateParams, $log, $cookies, $window, $filter, logonService, requestService) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'requestController';

        vm.closeErrorAlert = function () {
            vm.submittingRequest = false;
            vm.hasErrors = false;
        }

        vm.submitRequest = function () {
            vm.submittingRequest = true;
            vm.hasErrors = false;
            vm.errorList = [];

            if (vm.form.$invalid)
            {
                vm.hasErrors = true;
                return;
            }            

            requestService.submitRequest(vm.request)
                .then(function (data) {
                    vm.submittingRequest = false;
                    if (data.hasErrors === true) {
                        vm.hasErrors = true;
                        $log.error('submitRequest: ' + JSON.stringify(data.errorList));
                        for (var count = 0 ; count < data.errorList.length ; count++) {
                            vm.errorList.push(data.errorList[0]);
                        }
                    }
                    else {
                        vm.main = data.data;
                    }
                })
                .catch(function (error) {
                    vm.submittingRequest = false;
                    vm.hasErrors = true;
                    $log.error('submitRequest: ' + JSON.stringify(error));                    
                    vm.errorList.push('Error submitting request');
                })
        }

        vm.newRequest = function () {
            $window.location.reload();
        }

        vm.orderDetailSetup = function (utilityType, initialize) {

            if (initialize === true) {
                vm.request.orderDetails = [];
            }            

            switch(utilityType) {
                case 'Gas':
                    vm.request.orderDetails.push({
                        utilityType: 'Gas',
                        firstName: vm.request.lead.firstName,
                        lastName: vm.request.lead.lastName,
                        address: vm.request.lead.address,
                        address2: vm.request.lead.address2,
                        city: vm.request.lead.city,
                        state: vm.request.lead.state,
                        zip: vm.request.lead.zip
                    });                    
                    vm.getUtilityList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length - 1);
                    //$log.info('orderDetails GAS: ' + JSON.stringify(vm.request.orderDetails));
                    //vm.getProgramList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length - 1);
                    
                    break;
                case 'Electric':
                    vm.request.orderDetails.push({
                        utilityType: 'Electric', firstName: vm.request.lead.firstName,
                        lastName: vm.request.lead.lastName,
                        address: vm.request.lead.address,
                        address2: vm.request.lead.address2,
                        city: vm.request.lead.city,
                        state: vm.request.lead.state,
                        zip: vm.request.lead.zip
                    });                    
                    vm.getUtilityList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length - 1);
                    //$log.info('orderDetails ELECTRIC: ' + JSON.stringify(vm.request.orderDetails));
                    //vm.getProgramList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length - 1);
                    break;
                case 'Dual Fuel':
                    vm.request.orderDetails.push({
                        utilityType: 'Gas', firstName: vm.request.lead.firstName,
                        lastName: vm.request.lead.lastName,
                        address: vm.request.lead.address,
                        address2: vm.request.lead.address2,
                        city: vm.request.lead.city,
                        state: vm.request.lead.state,
                        zip: vm.request.lead.zip
                    });                    
                    vm.getUtilityList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length - 1);
                    //$log.info('orderDetails DUEL FUEL GAS: ' + JSON.stringify(vm.request.orderDetails));
                    //vm.getProgramList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length - 1);
                    vm.request.orderDetails.push({
                        utilityType: 'Electric', firstName: vm.request.lead.firstName,
                        lastName: vm.request.lead.lastName,
                        address: vm.request.lead.address,
                        address2: vm.request.lead.address2,
                        city: vm.request.lead.city,
                        state: vm.request.lead.state,
                        zip: vm.request.lead.zip
                    });                    
                    vm.getUtilityList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length - 1);
                    //$log.info('orderDetails DUEL FUEL ELECTRIC: ' + JSON.stringify(vm.request.orderDetails));
                    //vm.getProgramList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length - 1);
                    break;
                default:
                    vm.hasErrors = true;
                    $log.error('orderDetailSetup: Invalid utility type (' + utilityType + ')');
                    vm.errorList.push('Invalid utility type (' + utilityType + ')');
            } 
        }

        vm.getIpLocation = function () {
            vm.gettingLocation = true;
            requestService.getIpLocation()
                .then(function (data) {
                    vm.gettingLocation = false;
                    vm.request.ipLocation = data;
                })
                .catch(function (error) {
                    vm.gettingLocation = false;
                    vm.hasErrors = true;
                    $log.error('getIpLocation: ' + JSON.stringify(error));
                    vm.errorList.push('Error retrieving IP location');
                });
        }

        vm.getZipcodeInfo = function (detail, formElement) {
            vm.gettingZip = true;
            //$log.info("requestController.getZipcodeInfo");
            //$log.info("form: " + form);
            requestService.getZipcodeInfo(detail.zip)
                .then(function (data) {
                    $log.info("requestService.getZipcodeInfo");
                    $log.info("data: " + JSON.stringify(data));
                    vm.gettingZip = false;
                    if (data[0].status) {
                        detail.zipErrorReason = data[0].reason;
                        formElement.$setValidity('zipcode', false);
                    }
                    else {
                        detail.zipErrorReason = null;
                        formElement.$setValidity('zipcode', true);
                        detail.city = data[0].city_states[0].city;
                        detail.state = data[0].city_states[0].state_abbreviation;
                    }                    
                })
                .catch(function (error) {
                    vm.gettingZip = false;
                    vm.hasErrors = true;
                    $log.error('getZipcodeInfo: ' + JSON.stringify(error));
                    vm.errorList.push('Error retrieving zip code information');
                })
        }

        vm.getBillingZipcodeInfo = function (detail, formElement) {
            vm.gettingBillingZip = true;
            //$log.info("requestController.getBillingZipcodeInfo");
            //$log.info("form: " + form);
            requestService.getZipcodeInfo(detail.billingZip)
                .then(function (data) {
                    $log.info("requestService.getZipcodeInfo");
                    $log.info("data: " + JSON.stringify(data));
                    vm.gettingBillingZip = false;
                    if (data[0].status) {
                        detail.billingZipErrorReason = data[0].reason;
                        formElement.$setValidity('zipcode', false);
                    }
                    else {
                        detail.billingZipErrorReason = null;
                        formElement.$setValidity('zipcode', true);
                        detail.billingCity = data[0].city_states[0].city;
                        detail.billingState = data[0].city_states[0].state_abbreviation;
                    }
                })
                .catch(function (error) {
                    vm.gettingBillingZip = false;
                    vm.hasErrors = true;
                    $log.error('getBillingZipcodeInfo: ' + JSON.stringify(error));
                    vm.errorList.push('Error retrieving zip code information');
                })
        }

        vm.getLead = function () {
            vm.gettingLead = true;
            //$log.info("requestController.getLead");
            requestService.getLead(vm.request.recordLocator, vm.request.user.vendorNumber)
                .then(function (data) {
                    //$log.info("requestService.getLead");
                    //$log.info("data: " + JSON.stringify(data));
                    vm.gettingLead = false;
                    vm.request.lead = data.data;
                    if (!vm.request.lead) {
                        //$log.info("no data" + vm.request.lead);
                        vm.form.RecordLocator.$setValidity('notfound', false);
                    }
                    else
                    {
                        //$log.info("data found" + vm.request.lead);
                        vm.form.RecordLocator.$setValidity('notfound', true);
                        //set initial phone number
                        vm.request.phone = vm.request.lead.phone;
                        vm.request.firstName = vm.request.lead.firstName;
                        vm.request.lastName = vm.request.lead.lastName;
                    }
                })
                .catch(function (error) {
                    vm.gettingLead = false;
                    vm.hasErrors = true;
                    $log.error('getLead: ' + JSON.stringify(error));
                    vm.errorList.push('Error retrieving lead information');
                })
        }

        vm.manualEntry = function () {
            vm.request.lead = {
                leadsId: 0,
                recordLocator: '',
                vendorNumber: '',
                firstName: '',
                lastName: '',
                address: '',
                address2: '',
                city: '',
                state: '',
                zip: '',
                phone: '',
                utility: ''
            };

            vm.request.phone = '';
            vm.request.firstName = '';
            vm.request.lastName = '';
        }

        vm.getProgramList = function (detailObject) {
            vm.gettingProgramList = true;
            //$log.info("requestController.getProgramList");
            //$log.info("requestController.getProgramList.detailObject: " + JSON.stringify(detailObject));
            requestService.getProgramList(detailObject.utility.utilityId, vm.request.user.vendorId, detailObject.utilityType)
                .then(function (data) {
                    //$log.info("requestService.getProgramList success");
                    //$log.info("data: " + JSON.stringify(data));
                    vm.gettingProgramList = false;
                    detailObject.programList = data.data;
                    if (detailObject.programList || detailObject.programList.length > 0) {
                        for (var p = 0; p < detailObject.programList.length; p++) {
                            var program = detailObject.programList[p];
                            program["detailString"] = program.programName
                                + ' (code: ' + program.programCode
                                + ', rate: ' + program.rate + '/' + program.unitOfMeasure.unitOfMeasureName
                                + ', etf: ' + ((program.etf != null) ? $filter('currency')(program.etf) : 'n/a')
                                + ', msf: ' + ((program.msf != null) ? $filter('currency')(program.msf) : 'n/a')
                                + ', term: ' + ((program.term != null) ? program.term : 'n/a') + ')';
                        }
                    }
                })
                .catch(function (error) {
                    vm.gettingProgramList = false;
                    vm.hasErrors = true;
                    $log.error('getProgramList: ' + JSON.stringify(error));
                    vm.errorList.push('Error retrieving program list');
                })
        }

        vm.getUtilityList = function (detailObject, index) {
            vm.gettingUtilityList = true;
            //$log.info("requestController.getUtilityList");
            //$log.info("requestController.getUtilityList.detailObject: " + JSON.stringify(detailObject));
            requestService.getUtilityList(vm.request.lead.state)
                .then(function (data) {
                    //$log.info("requestService.getUtilityList success");
                    //$log.info("data: " + JSON.stringify(data));
                    vm.gettingUtilityList = false;
                    detailObject.utilityList = data.data;
                    if (detailObject.utilityList || detailObject.utilityList.length > 0) {
                        for (var p = 0; p < detailObject.utilityList.length; p++) {
                            var utility = detailObject.utilityList[p];
                            utility["detailString"] = utility.name
                                + ' (' + utility.ldcCode + ')';
                        }
                    }
                })
                .catch(function (error) {
                    vm.gettingUtilityList = false;
                    vm.hasErrors = true;
                    $log.error('gettingUtilityList: ' + JSON.stringify(error));
                    vm.errorList.push('Error retrieving utility list');
                })
        }

        var getRelationshipList = function () {
            vm.gettingRelationshipList = true;
            requestService.getRelationshipList()
                .then(function (data) {
                    vm.gettingRelationshipList = false;
                    if (data.hasErrors === true) {                        
                        vm.hasErrors = true;
                        $log.error('getRelationshipList: ' + JSON.stringify(error));
                        for (var count = 0 ; count < data.errorList.length ; count++) {
                            vm.errorList.push(data.errorList[0]);
                        }                        
                    }
                    else {
                        vm.relationshipList = data.data;
                    }
                })
                .catch(function (error) {
                    vm.getRelationshipListgettingRelationshipList = false;
                    vm.hasErrors = true;
                    $log.error('getRelationshipList: ' + JSON.stringify(error));
                    vm.errorList.push('Error retrieving relationship list');
                })
        }

        vm.copyBilling = function (detail) {
            detail.billingFirstName = vm.request.firstName;
            detail.billingLastName = vm.request.lastName;
            detail.billingAddress = detail.address;
            detail.billingAddress2 = detail.address2;
            detail.billingCity = detail.city;
            detail.billingState = detail.state;
            detail.billingZip = detail.zip;
        }

        vm.programChange = function (detail) {
            //detail.accountNumberPattern = "^\\d{" + detail.program.accountNumberLength + "}$";
            detail.accountNumberPattern = "^\\w{" + detail.program.accountNumberLength + "}$";

            if (detail.program.meterNumber === true) {
                detail.meterNumberPattern = "^\\w{1," + detail.program.meterNumberLength + "}$";
            }
            else
            {
                detail.meterNumberPattern = "";
            }
        }

        vm.utilityChange = function (detail) {
            vm.getProgramList(detail);
        }

        activate();

        function activate() {
            vm.request = {};
            vm.request.orderDetails = [];

            //$log.info('logonService.getUser(): ' + JSON.stringify(logonService.getUser()));
            vm.request.user = $cookies.getObject('user');
            //$log.info('$cookies.getUser(): ' + JSON.stringify(vm.request.user));
            if (!vm.request.user) {
                $state.go('logon');
                //vm.user = {sparkId:'000002',agentId:'mjrae',firstName:'Mike',lastName:'Rae',ven
            }

            
            vm.gettingLead = false;
            vm.gettingLocation = false;
            vm.gettingProgramList = false;
            vm.gettingRelationshipList = false;
            vm.submittingRequest = false;
            vm.errorList = [];            
            
            //basic regex patterns
            vm.zipPattern = "^\\d{5}$";
            vm.statePattern = "^\[A-Za-z]{2}$";

            vm.getIpLocation();
            getRelationshipList();

            vm.userId = vm.request.user.userId;

        }

        
    }

})();