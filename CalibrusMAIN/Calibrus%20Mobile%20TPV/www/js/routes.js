"use strict";

angular.module('calibrus')
  .config(function ($stateProvider, $urlRouterProvider) {

    $stateProvider
      .state('logon', {
        url: '/logon',
        templateUrl: 'templates/logon.html',
        controller: 'LogonCtrl',
        controllerAs: 'vmLogon'
      })

      .state('app', {
        url: '/app',
        abstract: true,
        templateUrl: 'templates/menu.html',
        controller: 'AppCtrl',
        controllerAs: 'vmApp'
      })
      .state('app.home', {
        url: '/home',
        views: {
          'menuContent': {
            templateUrl: 'templates/home.html',
            controller: 'HomeCtrl',
            controllerAs: 'vmHome'
          }
        },
        data: {requiresLogin: true}
      })
      .state('app.data-entry', {
        abstract: true,
        url: '/data-entry',
        views: {
          'menuContent': {
            template: '<ion-nav-view></ion-nav-view>',
            controller: 'DataEntryCtrl',
            controllerAs: 'vmDataEntry'
          }
        },
        data: {requiresLogin: true}
      })
      .state('app.data-entry.authorizedParty', {
        url: '/authorized-party',
        views: {
          '@app.data-entry': {
            templateUrl: 'templates/data-entry/authorized-party.html'
          }
        },
        data: {requiresLogin: true}
      })
      .state('app.data-entry.line-item', {
        abstract: true,
        url: '/:lineItemIndex',
        template: '<ion-nav-view></ion-nav-view>',
        controller: 'DataEntryLineItemCtrl',
        controllerAs: 'vmDataEntryLineItem',
        data: {requiresLogin: true}
      })
      .state('app.data-entry.line-item.address', {
        url: '/address',
        views: {
          '@app.data-entry.line-item': {
            templateUrl: 'templates/data-entry/address.html',
            controller: 'DataEntryLineItemAddressCtrl'
          }
        },
        data: {requiresLogin: true}
      })
      .state('app.data-entry.line-item.address-gps', {
        url: '/address/gps',
        views: {
          '@app.data-entry.line-item': {
            templateUrl: 'templates/data-entry/address-gps.html',
            controller: 'DataEntryLineItemAddressGpsCtrl',
            controllerAs: 'vmDataEntryLineItemAddressGps'
          }
        },
        data: {requiresLogin: true}
      })
      .state('app.data-entry.line-item.address.manual', {
        url: '/address/manual',
        views: {
          '@app.data-entry.line-item': {
            templateUrl: 'templates/data-entry/address-manual.html'
          }
        },
        data: {requiresLogin: true}
      })
      .state('app.data-entry.line-item.utilities-and-programs', {
        url: '/utilities-and-programs',
        views: {
          '@app.data-entry.line-item': {
            templateUrl: 'templates/data-entry/utilities-and-programs.html',
            controller: 'DataEntryLineItemUtilitiesAndProgramsCtrl',
            controllerAs: 'vmDataEntryLineItemUtilitiesAndPrograms'
          }
        },
        data: {requiresLogin: true}
      })
      .state('app.data-entry.line-item.current-utilities', {
        url: '/current-utilities',
        views: {
          '@app.data-entry.line-item': {
            templateUrl: 'templates/data-entry/current-utilities.html',
            controller: 'DataEntryLineItemCurrentUtilitiesCtrl',
            controllerAs: 'vmDataEntryLineItemCurrentUtilities'
          }
        },
        data: {requiresLogin: true}
      })

      .state('app.data-entry.summary-and-signature', {
        url: '/summary-and-signature',
        views: {
          '@app.data-entry': {
            templateUrl: 'templates/data-entry/summary-and-signature.html',
            controller: 'DataEntrySummaryAndSignatureCtrl',
            controllerAs: 'vmDataEntrySummaryAndSignature'
          }
        },
        data: {requiresLogin: true}
      })

      .state('app.agent-info', {
        url: '/agent-info',
        views: {
          'menuContent': {
            templateUrl: 'templates/agent-info.html',
            controller: 'AgentInfoCtrl',
            controllerAs: 'vmAgentInfo'
          }
        },
        data: {requiresLogin: true}
      })

      .state('app.single', {
        url: '/page-1',
        views: {
          'menuContent': {
            templateUrl: 'templates/page-1.html',
            controller: 'HomeCtrl',
            controllerAs: 'single'
          }
        },
        data: {requiresLogin: true}
      });

    $urlRouterProvider.otherwise('/logon');
  });
