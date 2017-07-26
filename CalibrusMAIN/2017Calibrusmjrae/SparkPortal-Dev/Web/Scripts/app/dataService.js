(function () {

    'use strict';

    angular.module("portal")
        .factory("portalData", portalData);

    function portalData($http, $location, $log) {

        var portalService = {
            getRates: getRates,
            getVendors: getVendors,
            getOffices: getOffices,
            getUsers: getUsers,
            getUsersInactive: getUsersInactive,
            getUserLogs:getUserLogs,
            getUserType: getUserType,
            getVendorUsers: getVendorUsers,
            getOfficeUsers: getOfficeUsers,
            getVendorOffices: getVendorOffices,
            getVendorPrograms: getVendorPrograms,
            getUsersForVendor: getUsersForVendor,
            updateUserStatus: updateUserStatus,
            updateVendorStatus: updateVendorStatus,
            updateOfficeStatus: updateOfficeStatus,
            getReports: getReports,
            addUserNote: addUserNote
        };
        return portalService;

        ////////////////////////////////////////////////////////////////
        
        function getRates() {
            
           

            return $http.get('/api/rates/')
                .then(function (response) {
                    return response.data;
                });
        }

        function getVendors(active) {

            return $http.get('/api/vendors/' + active)
                .then(function (response) {
                    return response.data;
                });
        }

        function getOffices(active) {
           
            return $http.get('/api/offices/' + active)
                .then(function (response) {
                    return response.data;
                });
        }

        function getUsers(active) {
           $log.info(active)
            return $http.get('/api/users/userlist/A/0/' + active)
                .then(function (response) {
                    return response.data;
                });
        }

        function getUsersInactive() {
            return $http.get('/api/users/inactive/')
                .then(function (response) {
                    return response.data;
                });
        }

        function getUserLogs(id) {
            console.log(id);
            return $http.get('/api/users/userlogs/' + id)
            .then(function (response) {
                return response.data
            });
        }

        function getReports(securityLevel) {
            $log.info('Security Level: ' + securityLevel);
            return $http.get('/api/reports/' + securityLevel)
                .then(function (response) {
                    $log.info('portalService.getReports');
                    $log.info(JSON.stringify(response.data));
                    return response.data;
                });
        }

        function getUserType(id) {

          return $http.get('/api/usertype/' + id)
                .then(function (response) {
                    return response.data;
                });
        }

        function getVendorUsers(id,active) {

            $log.info("****getVendorUsers");
            $log.info("VENDORID: " + id);

            return $http.get('/api/users/userlist/V/' + id +'/' + active)
                .then(function (response) {
                    $log.info("****getVendorUsers data: " + JSON.stringify(response.data));
                    return response.data;
                });
        }

        function getOfficeUsers(officeId, active) {
            var query; $log.info("****getOfficeUsers");
            $log.info("OFFICEID: " + officeId);
            if (active === undefined)
            {
                query = 'api/users/userlist/O/' + officeId;
            } else
            {
                query = '/api/users/userlist/O/' + officeId + '/' + active;
            }
            return $http.get(query).then(function (response) { return response.data; });
        }

        //function getOfficeUsers(officeId, active) {

        //    $log.info("****getOfficeUsers");
        //    $log.info("OFFICEID: " + officeId);

        //    return $http.get('/api/users/userlist/O/' + officeId + '/' + active)
        //        .then(function (response) {
        //            return response.data;
        //        });
        //}

        function getVendorOffices(id, active) {

            
          
            $log.info("VENDORID: " + id);

            return $http.get('/api/offices/' + id + '/' + active)
                .then(function (response) {
                    return response.data;
                });
        }

        function getVendorPrograms(id) {

          
            $log.info("VENDORID: " + id);

            return $http.get('/api/rates/' + id)
                .then(function (response) {
                    return response.data;
                });
        }

        function getUsersForVendor(id) {

            

            return $http.get('/api/users/' + id)
                .then(function (response) {
                    return response.data;
                });
        }

        function updateUserStatus(userId, reason, loggedInUser) {
            

            $log.info("updateUserStatus userId: " + userId);
            $log.info("updateUserStatus reason: " + reason);
            $log.info("updateUserStatus loggedInUser: " + loggedInUser);

            return $http.post('/api/Users/', { UserId: userId, Reason: reason, LoggedInUser: loggedInUser }).
                then(function(data, status, headers, config) {
                    $log.info("updateUserStatus: SUCCESS");
                });

        }

        function addUserNote(userId, reason, loggedInUser) {

            return $http.post('/api/users/note/', { UserId: userId, Reason: reason, LoggedInUser: loggedInUser }).
                then(function (data, status, headers, config) {
                    $log.info("addUserLog: SUCCESS");
                });
        }

        function updateVendorStatus(vendorId, loggedInUser) {


            $log.info("updateUserStatus vendorId: " + vendorId);
            $log.info("updateUserStatus loggedInUser: " + loggedInUser);

            return $http.post('/api/Vendors/', { VendorId: vendorId, LoggedInUser: loggedInUser }).
                then(function (data, status, headers, config) {
                    $log.info("updateVendorStatus: SUCCESS");
                });
        }

        function updateOfficeStatus(officeId, loggedInUser) {


            $log.info("updateUserStatus officeId: " + officeId);
            $log.info("updateUserStatus loggedInUser: " + loggedInUser);

            return $http.post('/api/Offices/', { OfficeId: officeId, LoggedInUser: loggedInUser }).
                then(function (data, status, headers, config) {
                    $log.info("updateOfficeStatus: SUCCESS");
                });

        }

        
        
    }

}());
