(function () {
    "use strict";

    var algoliaController = function ($location, toastr) {
        var vm = this;

        var client = algoliasearch('SCVMS5FE3S', '4f9af7603070818440603e882fa48cbf');

        // var index = client.initIndex('contacts');
        // index.search('jimmie', function (err, content) {
        //     console.log(content.hits);
        // });

        vm.search = function (val) {
            var index = client.initIndex('boomerang');
            index.search("HDR", function (err, content) {
                vm.algoliaResult = {};
                vm.algoliaResult = content.hits;
                console.log(content.hits);
            });
        };

        vm.search();


    }

    angular.module('calibrus').controller('algoliaController', algoliaController);
}());