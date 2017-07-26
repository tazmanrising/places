"use strict";

angular.module('calibrus')
  .service('enrollmentService', function (userCache) {
    var _this = this;
    var order;
    //order.authorizedParty.creditCheck = 1;
    //var messages = {};
    //messages.list = [];
    //messages.list = "blah";
    //messages.list = "update";
    //console.log('messages',messages);

    var restore = function () {
      order = userCache.get('order');
      if (!order) _this.resetEnrollment();
      if (!order.lineItems) order.lineItems = [];
    };

    this.defaults = {
      authorizedParty: null,
      lineItem: {
        serviceLocation: null,
        additionalLocations: [],
        services: {
          electric: {selectedProgramId: null, utilityAndProgram: null, serviceProvider: null},
          naturalGas: {selectedProgramId: null, utilityAndProgram: null, serviceProvider: null}
        }
      },
      contactPreference: 'phone',
      currentService: {
        billingAddressSameAsServiceAddress: true,
        nameSameAsAuthorizedParty: true
      }
    };

    this.setAuthorizedParty = function (authorizedParty) {
      order.authorizedParty = authorizedParty;
      
       //order.authorizedParty.creditCheck = 1;
      _this.saveEnrollment();
      _this.setContactPreference(order.contactPreference);
    };

    this.setAuthorizedPartyFromLead = function (lead) { // NOTE : Might be able to do something with lead.utility
      if (!order.authorizedParty)  order.authorizedParty = {};

      order.authorizedParty.creditCheck = false;
      order.authorizedParty.firstName = lead.firstName;
      order.authorizedParty.lastName = lead.lastName;

      var phone;
      if (lead.phone) {
        phone = lead.phone.replace(/[^\d]/, '');
        if (phone.length === 10) {
          order.authorizedParty.phone = "(" + phone.substr(0, 3) + ") " + phone.substr(3, 3) + "-" + lead.phone.substr(6, 4);
        }
      }

      var lineItem = _this.getLineItem(0);
      lineItem.serviceLocation = {
        address1: lead.address,
        address2: lead.address2,
        city: lead.city,
        state: lead.state,
        zip: lead.zip
      };

      _this.saveEnrollment();
    };

    this.getAuthorizedParty = function () {
      //console.log('order.authorizedParty',order.authorizedParty);
      return order.authorizedParty;
    };

    this.getLineItem = function (index) {
      if (!order.lineItems[index]) order.lineItems[index] = angular.copy(_this.defaults.lineItem);
      
      //console.log('index', index);

      return order.lineItems[index];
    };

    this.setSignature = function (signature) {
      order.signature = signature;
      _this.saveEnrollment();
    };

    this.saveEnrollment = function () {
      order.isNew = false;
      userCache.put('order', order);
    };

    this.getEnrollment = function () {
      return angular.copy(order);
    };

    this.hasCachedEnrollment = function () {
      var order = userCache.get('order');
      return !(order && order.isNew);
    };

    this.setContactPreference = function (contactPreference) {
      switch (contactPreference) {
        case 'text':
          if (!(order.authorizedParty && order.authorizedParty.phoneIsMobile)) contactPreference = _this.defaults.contactPreference;
        case 'phone':
        case 'email':
          order.contactPreference = contactPreference;
          break;
        default:
          order.contactPreference = _this.defaults.contactPreference;
          break;
      }
      _this.saveEnrollment();
    };

    this.resetEnrollment = function () {
      order = {
        lineItems: [],
        authorizedParty: _this.defaults.authorizedParty,
        contactPreference: _this.defaults.contactPreference,
        signature: null,
        isNew: true
      };
      userCache.put('order', order);
    };

    this.enrollmentToCalibrusReqeust = function () {
      return {
        phone: order.authorizedParty.phone.replace(/[^\d]/gi, ''),
        firstName: order.authorizedParty.firstName,
        lastName: order.authorizedParty.lastName,
        lead: {
          recordLocator: '',
          leadsId: 0
        },
        orderDetails: order.lineItems.reduce(function (orderDetailsSoFar, lineItem) {
          var orderDetails = Object.keys(lineItem.services).map(function (serviceType) {
            var service = lineItem.services[serviceType];
            if (service.selectedProgramId === null && service.serviceProvider === null && service.utilityAndProgram === null) return;

            var isAddress = service.currentService.billingAddressSameAsServiceAddress;
            var isName = service.currentService.nameSameAsAuthorizedParty;
            return {
              relationship: service.currentService.relationshipToAuthorizedParty,
              accountNumber: service.currentService.accountNumber,
              program: {
                accountNumberType: {
                  accountNumberTypeName: service.utilityAndProgram.accountNumberTypeName,
                },
                programId: service.utilityAndProgram.programId
              },
              utilityType: service.utilityAndProgram.utilityTypeName,

              billingAddress: isAddress ? lineItem.serviceLocation.address1 : service.currentService.billingLocation.address1,
              billingAddress2: isAddress ? lineItem.serviceLocation.address2 : service.currentService.billingLocation.address2,
              billingCity: isAddress ? lineItem.serviceLocation.city : service.currentService.billingLocation.city,
              billingState: isAddress ? lineItem.serviceLocation.state : service.currentService.billingLocation.state,
              billingZip: isAddress ? lineItem.serviceLocation.zip : service.currentService.billingLocation.zip,

              billingFirstName: isName ? order.authorizedParty.firstName : service.currentService.firstName,
              billingLastName: isName ? order.authorizedParty.lastName : service.currentService.lastName,

              address: lineItem.serviceLocation.address1,
              address2: lineItem.serviceLocation.address2,
              city: lineItem.serviceLocation.city,
              state: lineItem.serviceLocation.state,
              zip: lineItem.serviceLocation.zip,

              meterNumber: service.currentService.meterNumber || '',
              serviceReference: service.currentService.serviceReference || ''
            };
          });

          orderDetails = orderDetails.filter(function (o) {
            return o;
          });

          return orderDetailsSoFar.concat(orderDetails);
        }, [])
      };
    };

    restore();
  });
