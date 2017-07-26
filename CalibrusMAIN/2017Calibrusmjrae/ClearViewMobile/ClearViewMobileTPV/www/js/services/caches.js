"use strict";

angular.module('calibrus')
  .service('globalHttpCache', function (CacheFactory) {
  // NOTE : Used to store data made to the api, since only specific calls will use this, it is possible to not clear this between users.
  return CacheFactory.createCache('globalHttpCache', {
    maxAge: 1 * 24 * 60 * 60 * 1000, // Items added to this cache expire after 1 day
    deleteOnExpire: 'aggressive',
    storageMode: 'localStorage'
  });
})
  .service('userHttpCache', function (CacheFactory) {
    // NOTE : Used to store USER specific data made to the api must be cleared on logout
    return CacheFactory.createCache('userHttpCache', {
      maxAge: 1 * 24 * 60 * 60 * 1000, // Items added to this cache expire after 1 day
      deleteOnExpire: 'aggressive',
      storageMode: 'localStorage'
    });
  })
  .service('globalCache', function (CacheFactory) {
    // NOTE : Used to store GLOBAL app setting NOT specific to a user! IE skip intro wizard
    return CacheFactory.createCache('globalCache', {
      maxAge: 30 * 24 * 60 * 60 * 1000, // Items added to this cache expire after 30 days.
      deleteOnExpire: 'aggressive',
      storageMode: 'localStorage'
    });
  })
  .service('userCache', function (CacheFactory) {
    // NOTE : Used to store specific information about this user, gets cleared on logout.
    return CacheFactory.createCache('userCache', {
      maxAge: 30 * 24 * 60 * 60 * 1000, // Items added to this cache expire after 30 days.
      deleteOnExpire: 'aggressive',
      storageMode: 'localStorage'
    });
  });
