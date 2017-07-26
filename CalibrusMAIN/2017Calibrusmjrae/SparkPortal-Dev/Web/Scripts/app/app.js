(function () {
    angular.module("portal", ["ui.bootstrap", "ui.mask", "portalFilters", "ngMessages", "highcharts-ng"])
    .run(function($http) {
        $http.defaults.headers.common['Auth-Token'] = 'U1BBUktUT0tFTg==';
    });
}());