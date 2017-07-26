'use strict';
var common = require('../common/controller');
var script = require('../common/scriptController');
var question = require('../common/questionController');
var libertyCtrl = require('./libertyController');


var express = require('express');
var router = express.Router();
var config = require('../../config.js')
var sqlConnection = config.database.liberty;
var sql = require('mssql');

var connection = new sql.ConnectionPool(sqlConnection, function (err) {
    // console.log(err)
});

router.get('/questions', function (req, res) {
    script.getQuestions(connection, req, res);
});

router.get('/scriptquestions', function (req, res) {
    libertyCtrl.getScriptQuestions(connection, req, res);
});

router.get('/marketstate', function (req, res){
    libertyCtrl.getMarketState(connection, req, res);
});

router.get('/marketutility/:id',function(req,res){
    libertyCtrl.getMarketUtility(connection, req, res);
});

router.get('/marketproduct/:id',function(req,res){
    libertyCtrl.getMarketProduct(connection, req, res);
});

router.post('/question', function (req, res) {
    console.log('question post', req)
    script.createQuestion(connection, req, res);
});

router.get('/directives', function (req, res) {
    console.log('get directives');
    question.getQuestionDirectives(connection, req, res);
});

router.get('/directiveassoc/:questionid', function (req, res) {
    console.log('get directiveassoc');
    script.getDirectiveAssoc(connection, req, res);
});

router.get('/questiondirectives/:id',function(req,res){
    libertyCtrl.getQuestionDirectives(connection, req, res);
});

router.get('/validateAgent/:id',function(req,res){
    console.log('api about to call getvalidAgent');
    common.getvalidAgent(connection, req, res);
});


router.put('/question', function (req, res) {
    script.ModifyQuestion(connection, req, res);
})

router.get('/scriptquestions/:statecode/:saleschannelid', function (req, res) {
    script.getScriptQuestions(connection, req, res);
})

router.post('/scriptquestion', function (req, res) {
    script.createScriptQuestion(connection, req, res);
})

router.put('/scriptquestion', function (req, res) {
    script.ModifyScriptQuestion(connection, req, res);
});

router.get('/main/:mainid', function (req, res) {
    var mainid = req.params.mainid;
    common.getmain(connection, mainid, req, res);
});

router.get('/btn/:btn', function (req, res) {
    var btn = req.params.btn;
    //  console.log(btn)
    common.btnCheck(connection, btn, req, res);
})

router.post('/question', function (req, res) {
    common.createQuestion(connection, req, res);
})

router.get('/states', function (req, res) {
    common.getStates(connection, req, res);
});

router.get('/saleschannel', function (req, res) {
    common.getSalesChannel(connection, req, res);
});


module.exports = router;