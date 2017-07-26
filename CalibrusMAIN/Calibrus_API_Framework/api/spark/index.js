/**
 * Created by sward on 5/3/2017.
 */
'use strict';
    var common = require('../common/controller')
    var spark = require('./config');
    var express = require('express');
    var router = express.Router();
    var config = require('../../config.js')
    var sqlConnection = config.database.spark;
    var sql = require('mssql');

    var connection = new sql.ConnectionPool(sqlConnection, function (err) {
       // console.log(err)
    })

    router.get('/main/:mainid',function(req,res){
    common.getmain(connection,req,res,spark.btn);
    });

    router.get('/btn/:btn',function(req,res){
        var btn = req.params.btn;
      //  console.log(btn)
        common.btnCheck(connection,btn,req,res,spark.btn)
    })

    router.post('/question',function(req,res){
        common.createQuestion(connection,req,res)
    })

module.exports = router ;
