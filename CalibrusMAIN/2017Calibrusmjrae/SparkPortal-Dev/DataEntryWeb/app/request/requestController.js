(function () {
    'use strict';

    angular
        .module('app')
        .controller('requestController', requestController);

    requestController.$inject = ['$location', '$state', '$stateParams', '$log', '$cookies', '$window', '$filter', 'logonService', 'requestService'];

    function requestController($location, $state, $stateParams, $log, $cookies, $window, $filter, logonService, requestService) {
        /* jshint validthis:true */
        var vm = this;
        vm.title = 'requestController';
        vm.creditcheck = true;
        vm.premisetype = 1;
        

        vm.closeErrorAlert = function () {
            vm.submittingRequest = false;
            vm.hasErrors = false;
        }

        vm.submitRequest = function (clone) {
            vm.submittingRequest = true;
            vm.hasErrors = false;
            vm.errorList = [];

            if (vm.form.$invalid) {
                vm.hasErrors = true;
                return;
            }

            if (clone) {
                // vm.request = {}
                vm.request.OrderDetails = []
                vm.request.FirstName = clone[0].authorizationFirstName;
                vm.request.LastName = clone[0].authorizationLastName;
                vm.request.Phone = clone[0].btn;
                vm.request.Lead = {};
                //vm.request.Lead.leadsId = -1;
                //vm.request.Lead.recordLocator = 0; ??


                clone.forEach(function (d) {
                    var detail = {}
                    detail.Program = {}
                    detail.Program.AccountNumberType = d.accountType
                    detail.Program.AccountNumberTypeName = ""
                    detail.AccountNumber = d.accountNumber
                    detail.BillingAddress = d.billingAddress
                    detail.BillingAddress2 = "";
                    detail.BillingCity = d.billingCity
                    detail.BillingState = d.billingState
                    detail.BillingZip = d.billingZip
                    detail.BillingFirstName = d.billingFirstName
                    detail.BillingLastName = d.billingLastName
                    detail.Address = d.serviceAddress
                    detail.Address2 = ""
                    detail.City = d.serviceCity
                    detail.State = d.serviceState
                    detail.Zip = d.serviceZip
                    detail.MeterNumber = d.meterNumber
                    detail.ServiceReference = d.serviceReferenceNumber
                    detail.Program.ProgramId = d.programId

                    vm.request.OrderDetails.push(detail);

                })
            }

            //$log.info('Clone Object: ' + JSON.stringify(vm.request));

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

            switch (utilityType) {
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
                    if (vm.recordLocator) {
                        vm.getUtilityList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length - 1);
                    }
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
                    if (vm.recordLocator) {
                        vm.getUtilityList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length -1);
                    }
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
                    if (vm.recordLocator) {
                        vm.getUtilityList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length - 1);
                    }                    //$log.info('orderDetails DUEL FUEL GAS: ' + JSON.stringify(vm.request.orderDetails));
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
                    if (vm.recordLocator) {
                             vm.getUtilityList(vm.request.orderDetails[vm.request.orderDetails.length - 1], vm.request.orderDetails.length - 1);
                      }
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
            $log.info('i want some ip info')
            vm.gettingLocation = true;
            requestService.getIpLocation()
                .then(function (data) {
                    $log.info(data)
                    vm.gettingLocation = false;
                    vm.request.ipLocation = data;
                })
                .catch(function (error) {
                    vm.gettingLocation = false;
                    vm.request.ipLocation = {ip: '0.0.0.0', city: 'unknown', region: 'unknown', country: 'unknown'};
                    $log.error('getIpLocation: ' + JSON.stringify(error));
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
                        vm.request.lead.zip = detail.zip
                        vm.request.lead.state = data[0].city_states[0].state_abbreviation;
                        vm.getUtilityList(detail);
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

        vm.getCloneZipcodeInfo = function (detail, formElement) {
            vm.gettingZip = true;
            //$log.info("requestController.getCloneZipcodeInfo");
            //$log.info("form: " + form);
            requestService.getZipcodeInfo(detail.serviceZip)
                .then(function (data) {
                    $log.info("requestService.getZipcodeInfo");
                    $log.info("data: " + JSON.stringify(data));
                    vm.gettingServiceZip = false;
                    if (data[0].status) {
                        detail.serviceZipErrorReason = data[0].reason;
                        formElement.$setValidity('serviceZip', false);
                    }
                    else {
                        detail.serviceZipErrorReason = null;
                        formElement.$setValidity('serviceZip', true);
                        detail.serviceCity = data[0].city_states[0].city;
                        detail.serviceState = data[0].city_states[0].state_abbreviation;
                    }
                })
                .catch(function (error) {
                    vm.gettingServiceZip = false;
                    vm.hasErrors = true;
                    $log.error('getZipcodeInfo: ' + JSON.stringify(error));
                    vm.errorList.push('Error retrieving service zip code information');
                })
        }

        vm.getLead = function () {
            vm.gettingLead = true;
            vm.request.accountType = 'Residential'; //leads are all residential orders

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
                    else {
                        //$log.info("data found" + vm.request.lead);
                        vm.form.RecordLocator.$setValidity('notfound', true);
                        //set initial phone number
                        vm.recordLocator = true;
                        vm.creditcheck = false;
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

        vm.getMainClone = function () {
            vm.MainClone = null;
            vm.gettingMainClone = true;
            //$log.info("requestController.getLead");

            requestService.getMainClone(vm.request.CalibrusRecordLocator.replace(/\D/g, ''))//strip all nonumeric values
                .then(function (data) {
                    //$log.info("requestService.getMainClone");
                    //$log.info("data: " + JSON.stringify(data));
                    vm.gettingMainClone = false;

                    if (data.length == 0) {
                        //$log.info("no data: " + data);
                        vm.form.CalibrusRecordLocator.$setValidity('notfound', false);
                    }
                    else if (data[0].verified == '1') {
                        //$log.info("Verified record: " + data);
                        vm.form.CalibrusRecordLocator.$setValidity('verified', false);
                    }
                    else {
                        vm.MainClone = data;
                        $log.info("data found: " + vm.MainClone);
                        vm.form.CalibrusRecordLocator.$setValidity('notfound', true);

                    }

                })
        }
        
        vm.manualEntry = function () {
            vm.request.manualOrder = true;

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

        //vm.getProgramList = function (detailObject) {
            vm.gettingProgramList = true;

        //    $log.info("requestController.getProgramList");
        //    $log.info("requestController.getProgramList.detailObject: " + JSON.stringify(detailObject));
        //    $log.info("Account Type: " + vm.request.accountType);

        //    requestService.getProgramList(detailObject.utility.utilityId, vm.request.user.vendorId, detailObject.utilityType, vm.request.accountType)
        //        .then(function (data) {
        //            //$log.info("requestService.getProgramList success");
        //            //$log.info("data: " + JSON.stringify(data));
                    
        //            vm.gettingProgramList = false;
        //            detailObject.programList = data.data;
        //            if (detailObject.programList || detailObject.programList.length > 0) {
        //                for (var p = 0; p < detailObject.programList.length; p++) {
        //                    var program = detailObject.programList[p];
        //                    program["detailString"] = program.programName
        //                        + ' (code: ' + program.programCode
        //                        + ', rate: ' + program.rate + '/' + program.unitOfMeasure.unitOfMeasureName
        //                        + ', etf: ' + ((program.etf != null) ? $filter('currency')(program.etf) : 'n/a')
        //                        + ', msf: ' + ((program.msf != null) ? $filter('currency')(program.msf) : 'n/a')
        //                        + ', term: ' + ((program.term != null) ? program.term : 'n/a') + ')';
        //                }
        //            }
        //        })
        //        .catch(function (error) {
        //            vm.gettingProgramList = false;
        //            vm.hasErrors = true;
        //            $log.error('getProgramList: ' + JSON.stringify(error));
        //            vm.errorList.push('Error retrieving program list');
        //        })
        //}

        vm.getUtilityList = function (detailObject, index) {
            vm.gettingUtilityList = true;
            //$log.info("requestController.getUtilityList");
            //$log.info("requestController.getUtilityList.detailObject: " + JSON.stringify(detailObject));
            // requestService.getUtilityList(vm.request.user.vendorId, detailObject.utilityType, vm.request.accountType, vm.request.lead.state)
            console.log("URL-string", vm.request.user.vendorId, vm.request.user.officeId, vm.request.lead.state, vm.request.lead.zip, vm.creditcheck, vm.premisetype);
            requestService.getProgramUtility(vm.request.user.vendorId, vm.request.user.officeId, vm.request.lead.state, vm.request.lead.zip, vm.creditcheck, vm.premisetype)
                .then(function (data) {
                    //$log.info("requestService.getUtilityList success");
                    $log.info("data: " + JSON.stringify(data));
                    vm.gettingUtilityList = false;
                    detailObject.utilityList = unique(data);
                    detailObject.programList = displayString(data);
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
                    vm.gettingRelationshipList = false;
                    vm.hasErrors = true;
                    $log.error('getRelationshipList: ' + JSON.stringify(error));
                    vm.errorList.push('Error retrieving relationship list');
                })
        }

        vm.getTitleList = function () {
            vm.gettingTitleList = true;
            requestService.getTitleList()
                .then(function (data) {
                    vm.gettingTitleList = false;
                    if (data.hasErrors === true) {
                        vm.hasErrors = true;
                        $log.error('getTitleList: ' + JSON.stringify(error));
                        for (var count = 0 ; count < data.errorList.length ; count++) {
                            vm.errorList.push(data.errorList[0]);
                        }
                    }
                    else {
                        vm.titleList = data.data;
                    }
                })
                .catch(function (error) {
                    vm.gettingTitleList = false;
                    vm.hasErrors = true;
                    $log.error('getTitleList: ' + JSON.stringify(error));
                    vm.errorList.push('Error retrieving title list');
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

        vm.copyBillingClone = function (detail) {
            detail.billingFirstName = detail.authorizationFirstName;
            detail.billingLastName = detail.authorizationLastName;
            detail.billingAddress = detail.serviceAddress;
            detail.billingCity = detail.serviceCity;
            detail.billingState = detail.serviceState;
            detail.billingZip = detail.serviceZip;
        }

        vm.programChange = function (detail) {
           
            detail.program.accountNumberType = {};
            detail.program.accountNumberType.accountNumberTypeName = detail.program.accountNumberTypeName;
            accountNumberPatternGenerator(detail);

            if (detail.program.meterNumber === true) {
                detail.meterNumberPattern = "^\\w{1," + detail.program.MeterNumberLength + "}$";
            }
            else {
                detail.meterNumberPattern = "";
            }
        }

        vm.utilityChange = function (detail) {
            vm.getProgramList(detail);
        }

        function accountNumberPatternGenerator(detail) {
           // console.log(detail.utility.ldcCode);
            var regexLength = detail.program.accountNumberLength;
            var prefix = "";

            if (detail.utilityType == "Gas") {
                if (detail.utility.ldcCode == "NYSEG") {
                    regexLength = regexLength - 3;
                    prefix = "N02";
                    detail.accountNumberPattern = "^([Nn]02)(\\w{" + regexLength + "})$";
                }
                else if (detail.utility.ldcCode == "RG&E") {
                    regexLength = regexLength - 3;
                    prefix = "R02";
                    detail.accountNumberPattern = "^([Rr]02)(\\w{" + regexLength + "})$";
                }
                else if (detail.utility.ldcCode == "PSEG") {
                    regexLength = regexLength - 2;
                    prefix = "PG";
                    detail.accountNumberPattern = "^([pP][gG])(\\w{" + regexLength + "})$";
                }
                else {
                    prefix = null;
                    detail.accountNumberPattern = "^(\\w{" + regexLength + "})$";
                }
            }
            else {
                if (detail.utility.ldcCode == "NYSEG") {
                    regexLength = regexLength - 3;
                    prefix = "N01";
                    detail.accountNumberPattern = "^([Nn]01)(\\w{" + regexLength + "})$";
                }
                else if (detail.utility.ldcCode == "RG&E") {
                    regexLength = regexLength - 3;
                    prefix = "R01";
                    detail.accountNumberPattern = "^([Rr]01)(\\w{" + regexLength + "})$";
                }
                else if (detail.utility.ldcCode == "PSEG") {
                    regexLength = regexLength - 2;
                    prefix = "PE";
                    detail.accountNumberPattern = "^([pP][eE])(\\w{" + regexLength + "})$";
                }
                else {
                    prefix = null;
                    detail.accountNumberPattern = "^(\\w{" + regexLength + "})$";
                }
            }

            if (prefix) {
                detail.accountNumberPatternErrorMessage = "Account Number is invalid. Must start with " + prefix + " followed by " + regexLength + " digits."
            }
            else {
                detail.accountNumberPatternErrorMessage = "Account Number is invalid. Must be " + regexLength + " characters."
            }

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

        function displayString(data) {
           
            data.forEach(function (program) {
                program.display = program.name + ' ' + program.programName
                 + ' (code: ' + program.programId
                                        + ', rate: ' + program.rate + '/' + program.unitOfMeasureName
                                        + ', etf: ' + ((program.etf != null) ? $filter('currency')(program.etf) : 'n/a')
                                        + ', msf: ' + ((program.msf != null) ? $filter('currency')(program.msf) : 'n/a')
                                        + ', term: ' + ((program.term != null) ? program.term : 'n/a') + ')';

            })
            return data;
        }


        function unique(data) { 
            var uList = [];
            var name ='';
           // uList.push(data[0].name);
            // electric
            data.forEach(function (utility) {
                if (utility.name != name && utility.utilityTypeName == 'Electric') {
                    uList.push({name:utility.name, type:'Electric', ldcCode:utility.ldcCode});
                    name = utility.name;                
                }
            });
            // gas
            name = '';
            data.forEach(function (utility) {
                if (utility.name != name && utility.utilityTypeName == 'Gas') {
                    uList.push({ name: utility.name, type: 'Gas', ldcCode: utility.ldcCode });
                    name = utility.name;                
                }
            });
            console.log(uList);
            return uList;
        }
    }

})();