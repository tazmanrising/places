"use strict";

angular.module('calibrus')
  .service('enrollmentService', function (userCache) {
    var _this = this,
      order;

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
      _this.saveEnrollment();
      _this.setContactPreference(order.contactPreference);
    };

    this.getAuthorizedParty = function () {
      return order.authorizedParty;
    };

    this.getLineItem = function (index) {
      if (!order.lineItems[index]) order.lineItems[index] = angular.copy(_this.defaults.lineItem);
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
