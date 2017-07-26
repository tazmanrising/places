'use strict';

var calibrusCtrl = require('./calibrusController');

var express = require('express');
var router = express.Router();
var config = require('../../config.js')
var sqlConnection = config.database.calibrus;
var sql = require('mssql');
//var diskspace = require('diskspace');

//diskspace.check('C', function (err, result)
// diskspace.check('\\10.100.40.210\e', function (err, result)
// {
//    console.log('diskspace: ',result.free)
// }, function(err){
//     console.log('err diskspace', err);
// });

var connection = new sql.ConnectionPool(sqlConnection, function (err) {
    // console.log(err)
});

router.get('/calibrusapplog', function (req, res) {
    calibrusCtrl.getCalibrusAppLog(connection, req, res);
});



module.exports = router;