"use strict";

angular.module('clearviewtpv')
  .config(function ($stateProvider, $urlRouterProvider) {

 
    $stateProvider
      // .state('app', {
      //   url: '/app',
      //   abstract: true,
      //   templateUrl: 'templates/menu.html',
      //   controller: 'AppCtrl',
      //   controllerAs: 'vmApp'
      // })

     .state('thankyou', {
       url:'/thankyou',
       templateUrl: 'templates/thankyou.html',
       controller: 'thankyouCtrl',
       controllerAs: 'vmThanks'
     })


      .state('consent', {
        url: '/consent',
        templateUrl: 'templates/consent.html',
        controller: 'consentCtrl',
        controllerAs: 'vmConsent'             
      })


      .state(':id',  {
         url: '/:id',
        templateUrl: 'templates/home.html',
        controller: 'consumerCtrl',
        controllerAs: 'vmHome'  
      });






    // .state('app.data-entry.summary-and-signature', {
    //   url: '/summary-and-signature',
    //   views: {
    //     '@app.data-entry': {
    //       templateUrl: 'templates/data-entry/summary-and-signature.html',
    //       controller: 'DataEntrySummaryAndSignatureCtrl',
    //       controllerAs: 'vmDataEntrySummaryAndSignature'
    //     }
    //   },
    //   data: {requiresLogin: true}
    // })

    // .state('app.single', {
    //   url: '/page-1',
    //   views: {
    //     'menuContent': {
    //       templateUrl: 'templates/page-1.html',
    //       controller: 'HomeCtrl',
    //       controllerAs: 'single'
    //     }
    //   },
    //   data: {requiresLogin: true}
    // });

    //$urlRouterProvider.otherwise(':id');
  });
