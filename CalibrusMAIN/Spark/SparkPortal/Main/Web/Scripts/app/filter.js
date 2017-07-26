(function() {

    angular.module('portalFilters', [])
        .filter('phoneNumber',
            function($log) {
                return function (input) {

                    $log.info('input: ' + input);
                    if (!input) { return ''; }

                    var value = input.toString().trim().replace(/^\+/, '');

                    if (value.match(/[^0-9]/)) {
                        return input;
                    }

                    var tel = value.match(/^(\d{3})(\d{3})(\d{4})$/);
                    return !tel ? null : '(' + tel[1] + ') ' + tel[2] + '-' + tel[3];
                }
            });

}())