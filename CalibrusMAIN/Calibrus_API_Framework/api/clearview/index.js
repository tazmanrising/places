/**
 * Created by sward on 5/3/2017.
 */
'use strict';
var common = require('../common/controller')
var express = require('express');
var router = express.Router();
var config = require('../../config.js')
var sqlConnection = config.database.clearview;
var sql = require('mssql');


var connection = new sql.ConnectionPool(sqlConnection, function (err) {
    // console.log(err)
})

router.get('/test',function(req,res){
    common.getmain(connection,req,res);
});

module.exports = router ;