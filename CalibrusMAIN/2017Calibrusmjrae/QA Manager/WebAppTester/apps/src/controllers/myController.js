angular.module('testMaker').controller('MyCtrl', MyCtrl);

function MyCtrl($scope) {

    $scope.utilities = {};

    $scope.utilities.all = [{
        "programId": 1062,
        "name": "Atlantic City Electric",
        "utilityTypeName": "Electric",
        "programName": "Test Program 24",
        "rate": 0.0775,
        "term": 12,
        "serviceReference": false,
        "accountNumberTypeName": "Account Number",
        "accountNumberLength": 10,
        "msf": 4.95,
        "etf": 100,
        "unitOfMeasureName": "KwH",
        "meterNumberLength": null,
        "zip": "85281",
        "$$hashKey": "object:325"
    }, {
        "programId": 1063,
        "name": "Atlantic City Electric",
        "utilityTypeName": "Electric",
        "programName": "Test Program 12",
        "rate": 0.0875,
        "term": 24,
        "serviceReference": false,
        "accountNumberTypeName": "Account Number",
        "accountNumberLength": 10,
        "msf": 5.95,
        "etf": 150,
        "unitOfMeasureName": "KwH",
        "meterNumberLength": null,
        "zip": "85281",
        "$$hashKey": "object:326"
    }, {
        "programId": 1064,
        "name": "Atlantic City Electric",
        "utilityTypeName": "Gas",
        "programName": "Test Gas Program 12",
        "rate": 0.555,
        "term": 12,
        "serviceReference": false,
        "accountNumberTypeName": "Account Number",
        "accountNumberLength": 10,
        "msf": 1,
        "etf": 10,
        "unitOfMeasureName": "Therm",
        "meterNumberLength": 5,
        "zip": "85281",
        "$$hashKey": "object:333"
    }];


    console.log($scope.utilities.all);

}
