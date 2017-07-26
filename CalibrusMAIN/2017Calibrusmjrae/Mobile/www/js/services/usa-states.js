"use strict";

angular.module('calibrus')
  .service('usaStatesService', function () {
    var items = [
      {name: "Alabama", label: "Alabama", abbreviation: "AL"},
      {name: "Alaska", label: "Alaska", abbreviation: "AK"},
      {name: "American Samoa", label: "American Samoa", abbreviation: "AS", type: 'territory'},
      {name: "Arizona", label: "Arizona", abbreviation: "AZ"},
      {name: "Arkansas", label: "Arkansas", abbreviation: "AR"},
      {name: "Armed Forces Europe", label: "Armed Forces Europe", abbreviation: "AE", type: 'territory'},
      {name: "Armed Forces Pacific", label: "Armed Forces Pacific", abbreviation: "AP", type: 'territory'},
      {name: "Armed Forces the Americas", label: "Armed Forces the Americas", abbreviation: "AA", type: 'territory'},
      {name: "California", label: "California", abbreviation: "CA"},
      {name: "Colorado", label: "Colorado", abbreviation: "CO"},
      {name: "Connecticut", label: "Connecticut", abbreviation: "CT"},
      {name: "Delaware", label: "Delaware", abbreviation: "DE"},
      {name: "District of Columbia", label: "District of Columbia", abbreviation: "DC", type: 'federal-district'},
      {name: "Federated States of Micronesia", label: "Federated States of Micronesia", abbreviation: "FM", type: 'territory'},
      {name: "Florida", label: "Florida", abbreviation: "FL"},
      {name: "Georgia", label: "Georgia", abbreviation: "GA"},
      {name: "Guam", label: "Guam", abbreviation: "GU", type: 'territory'},
      {name: "Hawaii", label: "Hawaii", abbreviation: "HI"},
      {name: "Idaho", label: "Idaho", abbreviation: "ID"},
      {name: "Illinois", label: "Illinois", abbreviation: "IL"},
      {name: "Indiana", label: "Indiana", abbreviation: "IN"},
      {name: "Iowa", label: "Iowa", abbreviation: "IA"},
      {name: "Kansas", label: "Kansas", abbreviation: "KS"},
      {name: "Kentucky", label: "Kentucky", abbreviation: "KY"},
      {name: "Louisiana", label: "Louisiana", abbreviation: "LA"},
      {name: "Maine", label: "Maine", abbreviation: "ME"},
      {name: "Marshall Islands", label: "Marshall Islands", abbreviation: "MH", type: 'territory'},
      {name: "Maryland", label: "Maryland", abbreviation: "MD"},
      {name: "Massachusetts", label: "Massachusetts", abbreviation: "MA"},
      {name: "Michigan", label: "Michigan", abbreviation: "MI"},
      {name: "Minnesota", label: "Minnesota", abbreviation: "MN"},
      {name: "Mississippi", label: "Mississippi", abbreviation: "MS"},
      {name: "Missouri", label: "Missouri", abbreviation: "MO"},
      {name: "Montana", label: "Montana", abbreviation: "MT"},
      {name: "Nebraska", label: "Nebraska", abbreviation: "NE"},
      {name: "Nevada", label: "Nevada", abbreviation: "NV"},
      {name: "New Hampshire", label: "New Hampshire", abbreviation: "NH"},
      {name: "New Jersey", label: "New Jersey", abbreviation: "NJ"},
      {name: "New Mexico", label: "New Mexico", abbreviation: "NM"},
      {name: "New York", label: "New York", abbreviation: "NY"},
      {name: "North Carolina", label: "North Carolina", abbreviation: "NC"},
      {name: "North Dakota", label: "North Dakota", abbreviation: "ND"},
      {name: "Northern Mariana Islands", label: "Northern Mariana Islands", abbreviation: "MP", type: 'territory'},
      {name: "Ohio", label: "Ohio", abbreviation: "OH"},
      {name: "Oklahoma", label: "Oklahoma", abbreviation: "OK"},
      {name: "Oregon", label: "Oregon", abbreviation: "OR"},
      {name: "Pennsylvania", label: "Pennsylvania", abbreviation: "PA"},
      {name: "Puerto Rico", label: "Puerto Rico", abbreviation: "PR", type: 'territory'},
      {name: "Rhode Island", label: "Rhode Island", abbreviation: "RI"},
      {name: "South Carolina", label: "South Carolina", abbreviation: "SC"},
      {name: "South Dakota", label: "South Dakota", abbreviation: "SD"},
      {name: "Tennessee", label: "Tennessee", abbreviation: "TN"},
      {name: "Texas", label: "Texas", abbreviation: "TX"},
      {name: "Utah", label: "Utah", abbreviation: "UT"},
      {name: "Vermont", label: "Vermont", abbreviation: "VT"},
      {name: "Virgin Islands, U.S.", label: "Virgin Islands, U.S.", abbreviation: "VI", type: 'territory'},
      {name: "Virginia", label: "Virginia", abbreviation: "VA"},
      {name: "Washington", label: "Washington", abbreviation: "WA"},
      {name: "West Virginia", label: "West Virginia", abbreviation: "WV"},
      {name: "Wisconsin", label: "Wisconsin", abbreviation: "WI"},
      {name: "Wyoming", label: "Wyoming", abbreviation: "WY"}
    ];

    this.getStates = function () {
      return items.filter(function (stateOrTerritory) {
        return !stateOrTerritory.type;
      });
    };

    this.getStatesAndDistricts = function () {
      return items.filter(function (stateOrTerritory) {
        return !stateOrTerritory.type || (stateOrTerritory.type && stateOrTerritory.type === 'federal-district');
      });
    };

    this.getStateAndTerritories = function () {
      return items.filter(function (stateOrTerritory) {
        return !stateOrTerritory.type || (stateOrTerritory.type && stateOrTerritory.type === 'territory');
      });
    };

    this.getCodeFromName = this.getAbbreviationFromName = function (stateName) {
      var stateAbbreviation;
      if (stateName) {
        stateName = stateName.toLowerCase();
        var matches = items.filter(function (item) {
          return item.name.toLowerCase() === stateName;
        });
        if (matches.length === 1) stateAbbreviation = matches[0].abbreviation;
      }
      return stateAbbreviation;
    };

    this.getNameFromCode = this.getLabelFromAbbreviation = function (code) {
      var label;
      if (code && code.length === 2) {
        code = code.toUpperCase();
        var matches = items.filter(function (item) {
          return item.abbreviation === code;
        });
        if (matches.length === 1) {
          label = matches[0].label;
        }
      }
      return label;
    };
  })
  .filter('usaState', function (usaStatesService) {
    return usaStatesService.getNameFromCode;
  });
