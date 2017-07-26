/**
 * Created by sward on 5/3/2017.
 */
'use strict';
var config = require('../../config.js')
var sql = require('mssql');
var mongojs = require('mongojs');
var collections = ['tpvLog']
var db = mongojs(config.mongo, collections)


exports.getmain = function (connection, req, res) {

    var request = new sql.Request(connection);
    request.input('mainid', sql.Int, req.params.mainid)
    var query = 'select  * from v1.main ' +
        ' where mainid = @mainid';
    request.query(query).then(function (resultset) {
        //  console.log(resultset)
        res.json(resultset.recordset)
    }).catch(function (err) {
        res.json(err)
    })

}

exports.btnCheck = function (connection, btn, req, res, expiry) {

    var request = new sql.Request(connection);
    request.input('btn', sql.VarChar, btn);
    request.input('expiry', sql.Int, expiry)
    request.output('result', sql.VarChar);
    var query = "if exists (select * from v1.Main m where m.btn = @btn " +
        " and (m.CallDateTime > getdate() - @expiry )" +
        " and m.Verified = '1' ) set @result = 'true' else set @result = 'false' ";
    request.query(query).then(function (resultset) {
        // console.log(resultset);
        res.json(resultset.output)
    }).catch(function (err) {
        res.json(err)
    })


}

exports.getStates = function (connection, req, res) {
    var request = new sql.Request(connection);
    var query = 'select * from Scripts.States order by StateCode'
    request.query(query).then(function (resultset) {
        res.json(resultset.recordset);
    }).catch(function (err) {
        res.json(err)
    });
}

exports.getSalesChannel = function (connection, req, res) {
    var request = new sql.Request(connection);
    var query = 'select * from vwSalesChannel'
    request.query(query).then(function (resultset) {
        res.json(resultset.recordset);
    }).catch(function (err) {
        res.json(err)
    });
}



exports.getvalidAgent = function (connection, req, res, id) {
    //console.log('param id', req.params.id);
    
    
    var request = new sql.Request(connection);
    request.input('Agentid', sql.VarChar, req.params.id);
    var query = "select * from [liberty].[v1].[User] a " +
        " WHERE a.AgentId = @Agentid " +  //'mjrae'
        " and a.IsActive = 1 ";
    request.query(query).then(function (resultset) {
        //console.log('query', query);
        //console.log('resultset.recordset',resultset.recordset);
        res.json(resultset.recordset);
    }).catch(function (err) {
        console.log('getvalidagent', err);
        res.json(err);
    });


}





