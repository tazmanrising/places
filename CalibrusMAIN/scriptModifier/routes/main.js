/**
 * Created by sward on 10/6/2016.
 */
var express = require('express');
var router = express.Router();
var sql = require('mssql');

var config =  require('../config.js') ;
console.log(config)

var connection = new sql.Connection(config, function(err){
    console.log(err)

})

router.get('/:db', function(req,res,next){
    var fred = req.params.db
    var request = new sql.Request(connection);
    request.input('db',sql.VarChar(50),req.params.db) ;
    // console.log(request);
    request.execute('spMainEditor ',function(err,recordsets){
        res.json(recordsets);
    })
});

router.get('/:db/:table', function(req,res,next){
    var request = new sql.Request(connection);
    request.input('db',sql.VarChar(50),req.params.db) ;
    request.input('table',sql.VarChar(50),req.params.table) ;
    // console.log(request);
    request.execute('spMainEditor ',function(err,recordsets){
        res.json(recordsets);
    })
});

router.get('/:db/:table/:mainid',function(req,res) {
    var request = new sql.Request(connection);
    request.input('db', sql.VarChar(50), req.params.db);
    request.input('table', sql.VarChar(50), req.params.table);
    request.input('id', sql.Int, req.params.mainid);
    var SQL = 'Select * from '  + req.params.db + req.params.table ;
    var WHERE = ' Where MainId = ' + req.params.mainid;
    var query = SQL + WHERE
    //console.log(query)

    request.query(query,function(err,recordset){
        if(err) return res.json(err)
        res.json(recordset) ;
    })


});
router.post('/save', function(req,res,next){
        console.log('where is the loop %s', req.body.Loop);
        //console.log(req.body);
        var request = new sql.Request(connection);
        request.input('db',sql.VarChar(50),req.body.db.name) ;
        request.input('table',sql.VarChar(50),req.body.table) ;
        request.input('id', sql.Int, req.body.MainId);
        request.input('verified', sql.NVarChar(1) ,req.body.Verified);
        request.input('concern', sql.NVarChar(sql.MAX),req.body.Concern);
        request.input('concerncode', sql.NVarChar(sql.MAX),req.body.ConcernCode);
        request.input('userid', sql.Int, req.body.UserId);
        request.input('wavname', sql.VarChar(50), req.body.WavName);
        request.input('outboundwavname', sql.VarChar(50), req.body.OutboundWavName);
        //console.log('request', request);
        request.execute('spSaveMain',function(err,recordsets){
            console.log(err)
            console.log(recordsets)
            res.json({error:'none'});
        });

    
})

module.exports = router;