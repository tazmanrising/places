(function () {

    'use strict';

    angular.module("portal").controller("dashboardController", dashboardController);

    var verifiedChartFactory = function (t, type) {
        return {
            options: {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false
                },
                title: {
                    text: t
                },
                tooltip: {
                    pointFormat: '{series.name}: <b>{point.y} ' + type + '</b>'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '{point.percentage:.1f}%'
                        },
                        showInLegend: true
                    }
                },
                drilldown: {
                    series: [
                        {
                            type: 'pie',
                            name: 'Not Verified',
                            id: 'Not Verified',
                            data: []
                        }
                    ]

                },
                credits: false
            },
            series: [
                {
                    type: 'pie',
                    size: '90%',
                    innerSize: '50%',
                    name: 'Dispositions',
                    data: [
                        {
                            name: 'Verified',
                            y: 0,
                            //color: Highcharts.getOptions().colors[2],
                            drilldown: 'Verified',
                            colorByPoint: true

                        },
                        {
                            name: 'Not Verified',
                            y: 0,
                            //color: Highcharts.getOptions().colors[8],
                            drilldown: 'Not Verified',
                            colorByPoint: true,
                        }
                    ]
                }
            ],
            loading: true
        }
    }

    function dashboardController(reportService, userCtx, $log) {

        var vm = this;
        vm.user = userCtx;
        vm.range = 'M';
        vm.titleRange = 'MTD';

        var getVerifiedToday = function () {

            if (vm.user.userVendorId) {
                vm.vendorId = vm.user.userVendorId;
            }
            if (vm.user.userOfficeId) {
                vm.officeId = vm.user.userOfficeId;
            }

            reportService.getVerifiedChartSummary('D', vm.vendorId, vm.officeId).then(function (data) {
                var verifiedSummaryDay = data[0];
                if (verifiedSummaryDay.length === 0) {
                    vm.verifiedPieChartConfigD.series[0].data = [];
                }
                for (var x = 0; x < verifiedSummaryDay.length; x++) {
                    //$log.info('DAY: ' + JSON.stringify(verifiedSummaryDay[x]));
                    vm.verifiedPieChartConfigD.series[0].data[x].name = verifiedSummaryDay[x].Disposition;
                    vm.verifiedPieChartConfigD.series[0].data[x].y = verifiedSummaryDay[x].Calls;
                    vm.verifiedPieChartConfigD.series[0].data[x].drilldown = verifiedSummaryDay[x].Disposition;
                    if (verifiedSummaryDay[x].Disposition === 'Verified') {
                        vm.verifiedPieChartConfigD.series[0].data[x].color = Highcharts.getOptions().colors[2];
                    } else {
                        vm.verifiedPieChartConfigD.series[0].data[x].color = Highcharts.getOptions().colors[8];
                    }
                }
                //drilldown
                for (var y = 0; y < data[1].length; y++) {
                    //$log.info('DAY DETAIL: ' + JSON.stringify(data[1][y]));
                    vm.verifiedPieChartConfigD.options.drilldown.series[0].data[y] = { name: data[1][y].Disposition, y: data[1][y].Calls }
                }

                vm.verifiedPieChartConfigD.loading = false;

            }, function (reason) {
                vm.error = "Error getting disposition list";
            });

        }
        var getVerifiedWeek = function () {
            if (vm.user.userVendorId) {
                vm.vendorId = vm.user.userVendorId;
            }
            if (vm.user.userOfficeId) {
                vm.officeId = vm.user.userOfficeId;
            }

            reportService.getVerifiedChartSummary('W', vm.vendorId, vm.officeId).then(function (data) {
                var verifiedSummaryWeek = data[0];
                if (verifiedSummaryWeek.length === 0) {
                    vm.verifiedPieChartConfigW.series[0].data = [];
                }
                for (var x = 0; x < verifiedSummaryWeek.length; x++) {
                    //$log.info('WEEK: ' + JSON.stringify(verifiedSummaryWeek[x]));
                    vm.verifiedPieChartConfigW.series[0].data[x].name = verifiedSummaryWeek[x].Disposition;
                    vm.verifiedPieChartConfigW.series[0].data[x].y = verifiedSummaryWeek[x].Calls;
                    vm.verifiedPieChartConfigW.series[0].data[x].drilldown = verifiedSummaryWeek[x].Disposition;
                    if (verifiedSummaryWeek[x].Disposition === 'Verified') {

                        vm.verifiedPieChartConfigW.series[0].data[x].color = Highcharts.getOptions().colors[2];
                    } else {
                        vm.verifiedPieChartConfigW.series[0].data[x].color = Highcharts.getOptions().colors[8];
                    }
                }
                //drilldown
                for (var y = 0; y < data[1].length; y++) {
                    //$log.info('WEEK DETAIL: ' + JSON.stringify(data[1][y]));
                    vm.verifiedPieChartConfigW.options.drilldown.series[0].data[y] = { name: data[1][y].Disposition, y: data[1][y].Calls }
                }

                vm.verifiedPieChartConfigW.loading = false;

            }, function (reason) {
                vm.error = "Error getting disposition list";
            });
        }
        var getVerifiedMonth = function () {

            if (vm.user.userVendorId) {
                vm.vendorId = vm.user.userVendorId;
            }
            if (vm.user.userOfficeId) {
                vm.officeId = vm.user.userOfficeId;
            }

            reportService.getVerifiedChartSummary('M', vm.vendorId, vm.officeId).then(function (data) {
                var verifiedSummaryMonth = data[0];
                if (verifiedSummaryMonth.length === 0) {
                    vm.verifiedPieChartConfigM.series[0].data = [];
                }
                for (var x = 0; x < verifiedSummaryMonth.length; x++) {
                    //$log.info('MONTH: ' + JSON.stringify(verifiedSummaryMonth[x]));
                    vm.verifiedPieChartConfigM.series[0].data[x].name = verifiedSummaryMonth[x].Disposition;
                    vm.verifiedPieChartConfigM.series[0].data[x].y = verifiedSummaryMonth[x].Calls;
                    vm.verifiedPieChartConfigM.series[0].data[x].drilldown = verifiedSummaryMonth[x].Disposition;
                    if (verifiedSummaryMonth[x].Disposition === 'Verified') {

                        vm.verifiedPieChartConfigM.series[0].data[x].color = Highcharts.getOptions().colors[2];
                    } else {
                        vm.verifiedPieChartConfigM.series[0].data[x].color = Highcharts.getOptions().colors[8];
                    }
                }
                //drilldown
                for (var y = 0; y < data[1].length; y++) {
                    //$log.info('MONTH DETAIL: ' + JSON.stringify(data[1][y]));
                    vm.verifiedPieChartConfigM.options.drilldown.series[0].data[y] = { name: data[1][y].Disposition, y: data[1][y].Calls }
                }

                vm.verifiedPieChartConfigM.loading = false;

            }, function (reason) {
                vm.error = "Error getting disposition list";
            });
        }
        var getVerifiedYear = function () {

            if (vm.user.userVendorId) {
                vm.vendorId = vm.user.userVendorId;
            }
            if (vm.user.userOfficeId) {
                vm.officeId = vm.user.userOfficeId;
            }

            reportService.getVerifiedChartSummary('Y', vm.vendorId, vm.officeId).then(function (data) {
                //$log.info('DATA YEAR: ' + JSON.stringify(data));

                var verifiedSummaryYear = data[0];
                if (verifiedSummaryYear.length === 0) {
                    vm.verifiedPieChartConfigY.series[0].data = [];
                }
                for (var x = 0; x < verifiedSummaryYear.length; x++) {
                    //$log.info('YEAR SUMMARY: ' + JSON.stringify(verifiedSummaryYear[x]));
                    vm.verifiedPieChartConfigY.series[0].data[x].name = verifiedSummaryYear[x].Disposition;
                    vm.verifiedPieChartConfigY.series[0].data[x].y = verifiedSummaryYear[x].Calls;
                    vm.verifiedPieChartConfigY.series[0].data[x].drilldown = verifiedSummaryYear[x].Disposition;
                    if (verifiedSummaryYear[x].Disposition === 'Verified') {

                        vm.verifiedPieChartConfigY.series[0].data[x].color = Highcharts.getOptions().colors[2];
                    } else {
                        vm.verifiedPieChartConfigY.series[0].data[x].color = Highcharts.getOptions().colors[8];
                    }
                }
                //drilldown
                for (var y = 0; y < data[1].length; y++) {
                    //$log.info('YEAR DETAIL: ' + JSON.stringify(data[1][y]));
                    vm.verifiedPieChartConfigY.options.drilldown.series[0].data[y] = { name: data[1][y].Disposition, y: data[1][y].Calls }
                }

                vm.verifiedPieChartConfigY.loading = false;

            }, function (reason) {
                vm.error = "Error getting disposition list";
            });
        }

        vm.verifiedPieChartConfigD = new verifiedChartFactory('Verified Calls Today', 'calls');
        vm.verifiedPieChartConfigW = new verifiedChartFactory('Verified Calls WTD', 'calls');
        vm.verifiedPieChartConfigM = new verifiedChartFactory('Verified Calls MTD', 'calls');
        vm.verifiedPieChartConfigY = new verifiedChartFactory('Verified Calls YTD', 'calls');

        getVerifiedToday();
        getVerifiedWeek();
        getVerifiedMonth();
        getVerifiedYear();

        //***************************************************************Top Vendor/Office/User Charts**************************************************************

        vm.rangeChanged = function (range) {

            switch (range) {
                case 'D':
                    vm.titleRange = 'Today';
                    break;
                case 'W':
                    vm.titleRange = 'WTD';
                    break;
                case 'M':
                    vm.titleRange = 'MTD';
                    break;
                case 'Y':
                    vm.titleRange = 'YTD';
                    break;
            }

            vm.getTopCharts(range);
        }

        var getTopVendors = function (range) {

            vm.topVendorsloading = true;

            if (vm.user.userVendorId) {
                var vId = vm.user.userVendorId;
            }

            reportService.getTopVendors(range, vId)
                .then(function (data) {
                    vm.topVendorsStats = data;
                    vm.topVendorsloading = false;
                })
                .catch(function (error) {
                    vm.topVendorsloading = false;
                });
        }

        var getTopOffices = function (range) {

            vm.topOfficesloading = true;

            if (vm.user.userVendorId) {
                vm.vendorId = vm.user.userVendorId;
            }
            if (vm.user.userOfficeId) {
                vm.officeId = vm.user.userOfficeId;
            }

            reportService.getTopOffices(range, vm.vendorId, vm.officeId)
                .then(function (data) {
                    vm.topOfficesStats = data;
                    vm.topOfficesloading = false;
                })
                .catch(function (error) {
                    vm.topOfficesloading = false;
                });
        }

        var getTopUsers = function (range) {

            vm.topUsersloading = true;

            if (vm.user.userVendorId) {
                vm.vendorId = vm.user.userVendorId;
            }
            if (vm.user.userOfficeId) {
                vm.officeId = vm.user.userOfficeId;
            }

            reportService.getTopUsers(range, vm.vendorId, vm.officeId)
                .then(function (data) {
                    vm.topUsersStats = data;
                    vm.topUsersloading = false;
                })
                .catch(function (error) {
                    vm.topUsersloading = false;
                });
        }

        vm.getTopCharts = function (range) {

            vm.topVendorsStats = null;
            vm.topOfficesStats = null;
            vm.topUsersStats = null;

            vm.vendorVerifiedTotal = 0;
            vm.vendorNonVerifiedTotal = 0;
            vm.officeVerifiedTotal = 0;
            vm.officeNonVerifiedTotal = 0;
            vm.userVerifiedTotal = 0;
            vm.userNonVerifiedTotal = 0;

            getTopVendors(range);
            getTopOffices(range);
            getTopUsers(range);

        }

        vm.getTopCharts('M');


        /***************************************************************CHART SETUP********************************************************/

        // Radialize the colors
        Highcharts.getOptions().colors = Highcharts.map(Highcharts.getOptions().colors, function(color) {
            return {
                radialGradient: { cx: 0.5, cy: 0.3, r: 0.7 },
                stops: [
                    [0, color],
                    [1, Highcharts.Color(color).brighten(-0.3).get('rgb')] // darken
                ]
            };
        });

        //vm.verifiedPieChartConfig = {
        //    options: {
        //        chart: {
        //            plotBackgroundColor: null,
        //            plotBorderWidth: null,
        //            plotShadow: false
        //        },
        //        title: {
        //            text: 'Sales Activity YTD'
        //        },
        //        tooltip: {
        //            pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
        //        },
        //        plotOptions: {
        //            pie: {
        //                allowPointSelect: true,
        //                cursor: 'pointer',
        //                dataLabels: {
        //                    enabled: true,
        //                    format: '{point.percentage:.1f}%'
        //                },
        //                showInLegend: true
        //            }
        //        },
        //        drilldown: {
        //            series: [
        //                {
        //                    type: 'pie',
        //                    name: 'Not Verified',
        //                    id: 'Not Verified',
        //                    data: [
        //                        ['Incomplete', 45],
        //                        ['Failed', 26],
        //                        ['Did not order', 12],
        //                        ['Wrong Information', 8],
        //                        ['Could Not Understand', 6],
        //                        ['Others', 2]
        //                    ]
        //                }
        //            ]

        //        }
        //    },
        //    series: [
        //        {
        //            type: 'pie',
        //            size: '90%',
        //            innerSize: '40%',
        //            name: 'Dispositions',
        //            data: [
        //                {
        //                    name: 'Verified',
        //                    y: 0,
        //                    color: Highcharts.getOptions().colors[2],
        //                    drilldown: 'Verified',
        //                    colorByPoint: true

        //                },
        //                {
        //                    name: 'Not Verified',
        //                    y: 0,
        //                    color: Highcharts.getOptions().colors[8],
        //                    drilldown: 'Not Verified',
        //                    colorByPoint: true,
        //                }
        //            ]
        //        }
        //    ],
        //    loading: vm.verifiedSummaryYearIsLoading
        //}


        vm.marketPieChartConfig = {
            options: {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false
                },
                title: {
                    text: 'Market Summary'
                },
                tooltip: {
                    pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '<b>{point.name}</b>: {point.percentage:.1f} %',
                            style: {
                                color: (Highcharts.theme && Highcharts.theme.contrastTextColor) || 'black'
                            }
                        }
                    }
                }
            },
            series: [
                {
                    type: 'pie',
                    size: '80%',
                    innerSize: '40%',
                    name: 'Market',
                    data: [
                        ['Direct', 66.0],
                        ['Telesales', 20.0]
                    ]
                }
            ],


            loading: false
        }

        vm.nonVerifiedBreakdownChartConfig = {
            options: {
                chart: {
                    plotBackgroundColor: null,
                    plotBorderWidth: null,
                    plotShadow: false
                },
                title: {
                    text: 'Non-Verified Disposition Breakdown'
                },
                tooltip: {
                    pointFormat: '{series.name}: <b>{point.percentage:.1f}%</b>'
                },
                plotOptions: {
                    pie: {
                        allowPointSelect: true,
                        cursor: 'pointer',
                        dataLabels: {
                            enabled: true,
                            format: '{point.percentage:.1f}%'
                        },
                        showInLegend: true
                    }
                }
            },
            series: [
                {
                    type: 'pie',
                    size: '80%',
                    innerSize: '40%',
                    name: 'Dispositions',
                    data: [
                        ['Incomplete', 45.0],
                        ['Failed', 26.8],
                        {
                            name: 'Did Not Order',
                            y: 12.8,
                            sliced: true,
                            selected: true
                        },
                        ['Wrong Information', 8.5],
                        ['Could Not Understand', 6.2],
                        ['Others', 0.7]
                    ]
                }
            ],


            loading: false
        }

        vm.barChartConfig = {
            options: {
                chart: {
                    type: 'column'
                },
                title: {
                    text: 'Top Sales Agents'
                },
                subtitle: {
                    text: 'Direct Sales'
                },
                xAxis: {
                    categories: [
                        'Tom',
                        'Mike',
                        'Eric',
                        'Gary',
                        'Tamara'
                    ],
                    crosshair: true
                },
                yAxis: {
                    min: 0,
                    title: {
                        text: 'Sales'
                    }
                },
                tooltip: {
                    headerFormat: '<span style="font-size:10px">{point.key}</span><table>',
                    pointFormat: '<tr><td style="color:{series.color};padding:0">{series.name}: </td>' +
                        '<td style="padding:0"><b>{point.y:.1f} mm</b></td></tr>',
                    footerFormat: '</table>',
                    shared: true,
                    useHTML: true
                },
                plotOptions: {
                    column: {
                        pointPadding: 0.2,
                        borderWidth: 0
                    }
                },

            },
            series: [
                {
                    name: 'Today',
                    data: [5.9, 1.5, 2.4, 2.2, 4.0]

                }, {
                    name: 'WTD',
                    data: [20.6, 23.8, 17.5, 10.4, 18.0]

                }, {
                    name: 'MTD',
                    data: [48.9, 38.8, 39.3, 41.4, 47.0]

                }, {
                    name: 'YTD',
                    data: [80.4, 60.2, 75.5, 80.7, 71.6]

                }
            ]
        }

        vm.verifiedGuageChartConfig = {
            options: {
                chart: {
                    type: 'gauge'
                },
                pane: {
                    startAngle: -150,
                    endAngle: 150,
                    background: [
                        {
                            backgroundColor: {
                                linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                                stops: [
                                    [0, '#FFF'],
                                    [1, '#333']
                                ]
                            },
                            borderWidth: 0,
                            outerRadius: '109%'
                        }, {
                            backgroundColor: {
                                linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                                stops: [
                                    [0, '#333'],
                                    [1, '#FFF']
                                ]
                            },
                            borderWidth: 1,
                            outerRadius: '107%'
                        }, {
                            // default background

                        }, {
                            backgroundColor: '#DDD',
                            borderWidth: 0,
                            outerRadius: '105%',
                            innerRadius: '103%'
                        }
                    ]
                },

            },
            series: [
                {
                    data: [86],

                }
            ],
            title: {
                text: 'Verification Rate'
            },
            yAxis: {
                min: 0,
                max: 100,

                minorTickInterval: 'auto',
                minorTickWidth: 1,
                minorTickLength: 10,
                minorTickPosition: 'inside',
                minorTickColor: '#666',


                tickPosition: 'inside',
                tickLength: 10,
                tickColor: '#666',

                plotBands: [
                    {
                        from: 0,
                        to: 60,
                        color: '#DF5353' // red
                    },
                    {
                        from: 60,
                        to: 85,
                        color: '#DDDF0D' // yellow
                    },
                    {
                        from: 85,
                        to: 100,
                        color: '#55BF3B' // green
                    }
                ],
                lineWidth: 0,
                tickInterval: 5,
                tickPixelInterval: 400,
                tickWidth: 0,
                labels: {
                    y: 15
                }
            },
            loading: false
        }

        vm.chartConfig = {
            options: {
                chart: {
                    type: 'solidgauge'
                },
                pane: {
                    startAngle: -150,
                    endAngle: 150,
                    background: [
                        {
                            backgroundColor: {
                                linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                                stops: [
                                    [0, '#FFF'],
                                    [1, '#333']
                                ]
                            },
                            borderWidth: 0,
                            outerRadius: '109%'
                        }, {
                            backgroundColor: {
                                linearGradient: { x1: 0, y1: 0, x2: 0, y2: 1 },
                                stops: [
                                    [0, '#333'],
                                    [1, '#FFF']
                                ]
                            },
                            borderWidth: 1,
                            outerRadius: '107%'
                        }, {
                            // default background

                        }, {
                            backgroundColor: '#DDD',
                            borderWidth: 0,
                            outerRadius: '105%',
                            innerRadius: '103%'
                        }
                    ]
                },
           
            },
            series: [
            {
                name: 'Verification Rate',
                data: [80],
                tooltip: {
                    valueSuffix: '%'
                }
            }],
            title: {
                text: 'Verification Rate'
            },
            yAxis: {
                min: 0,
                max: 100,

                minorTickInterval: 'auto',
                minorTickWidth: 1,
                minorTickLength: 10,
                minorTickPosition: 'inside',
                minorTickColor: '#666',

                tickPosition: 'inside',
                tickLength: 10,
                tickColor: '#666',    
                plotBands: [
                        {
                            from: 0,
                            to: 60,
                            color: '#DF5353' // red
                        },
                        {
                            from: 60,
                            to: 85,
                            color: '#DDDF0D' // yellow
                        },
                        {
                            from: 85,
                            to: 100,
                            color: '#55BF3B' // green
                        }
                ],
                title: {
                    text: '%'
                },
                lineWidth: 0,
                tickInterval: 5,
                tickPixelInterval: 400,
                tickWidth: 0,
                labels: {
                    step: 2,
                    rotation: 'auto'
                },
            },
            loading: false
        }

    }

}());