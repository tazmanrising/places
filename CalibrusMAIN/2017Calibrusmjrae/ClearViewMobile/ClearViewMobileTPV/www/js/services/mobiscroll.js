"use strict";

angular.module('calibrus')
  .constant('mobiscrollTheme', 'calibrus')
  .service('mobiscrollService', function (mobiscrollTheme) {

    var display = 'bottom';
    var animate = 'slideveretical';

    this.birthday = {
      theme: mobiscrollTheme,
      display: display,
      animate: animate,
      dateFormat: 'mm/dd/yy',
      defaultValue: moment().subtract(20, 'year').toDate(),
      maxDate: new Date(),
      minDate: moment().subtract(120, 'year').toDate()
    };

    this.height = {
      theme: mobiscrollTheme,
      display: display,
      animate: animate,
      max: 230,
      wheels: [[
        {
          keys: [2, 3, 4, 5, 6, 7, 8],
          values: ["2'", "3'", "4'", "5'", "6'", "7'", "8'"]
        },
        {
          keys: [0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11],
          values: ['0"', '1"', '2"', '3"', '4"', '5"', '6"', '7"', '8"', '9"', '10"', '11"']
        }
      ]],
      formatResult: function (data) {
        return (data[0] && (data[1] || data[1] === 0)) ? data[0] + ' ft ' + data[1] + ' in' : '';
      },
      parseValue: function (value) {
        if (!value) {
          return null;
        }
        value = parseInt(value);
        return [Math.floor(value / 12), value % 12];
      },
      onInit: function (scroller) {
        setTimeout(function () {
          if (!scroller.getVal()) {
            scroller.setVal([3, 0], true);
          }
        }, 1);
      }
    };

    this.zip = {
      theme: mobiscrollTheme,
      display: display,
      animate: animate,
      allowLeadingZero: true,
      template: 'ddddd dddd',
      fill: 'ltr',
      placeholder: '',
      formatValue: function (val) {
        if (val.length === 9) {
          return (val.slice(0, 5).join('') + '-' + val.slice(5, 9).join(''));
        } else {
          return val.slice(0, 5).join('');
        }
      },
      parseValue: function (val) {
        if (val)
          val = val.replace(/\D/g, '').split('');
        return val;
      },
      validate: function (val, dir, inst) {
        if (inst._markup) {
          inst._markup.find('.' + inst.buttons.set.parentClass).toggleClass('disabled', !(val.length === 9 || val.length === 5 || val.length === 0));
        }
      }
    };

    this.select = {
      theme: mobiscrollTheme,
      display: display,
      animate: animate
    };

    this.selectMultiLine = {
      theme: mobiscrollTheme,
      display: display,
      animate: animate,
      multiline: 3,
      height: 50
    };

    this.phone = {
      theme: mobiscrollTheme,
      display: display,
      animate: animate,
      pattern: /^\(\d{3}\)\s\d{3}-\d{4}$/,
      template: '(ddd) ddd-dddd',
      placeholder: '',
      fill: 'ltr',
      formatValue: function (val) {
        if (val && !val.length) {
          return '';
        }

        return '(' + val.slice(0, 3).join('') + ') ' + val.slice(3, 6).join('') + '-' + val.slice(6, 10).join('');
      },
      parseValue: function (valueString) {
        if (!valueString) return valueString;
        return valueString.replace(/\D/g, '').split('');
      },
      validate: function (val, inst) {
        return val.length === 10 || val.length === 0;
      }
    };

    this.time = {
      theme: mobiscrollTheme,
      display: display,
      animate: animate
    };
  });
