var script = require('./common/transactionController.js')
var express = require('express');
var path = require('path');
var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var tsql = require("seriate");
var mongodb = require('mongodb').MongoClient;
var objectId = require('mongodb').ObjectID;
var mongoose = require('mongoose');
var Server = require('./models/mainModel');

var config = require('./config.js')
var sqlConnection = config.database.liberty;
var sql = require('mssql');
var connection = new sql.ConnectionPool(sqlConnection, function (err) {
  // console.log(err)

});

mongoose.connect('mongodb://cal:cal@10.100.40.204:27017/CalibrusServers');

// 1. get highest mainid inserted into mongodb
// 2. starting with that #,  >= mainid in mongo , search for new mainids in db main
// 3. also search for orderdetails  and then update main



objectId = "5923da330b3c3842702e44c8"; //5907bf972419a45b44bc7f05";

Server.findOne({_id: objectId},
  function (err, server) {
    if (err) throw err;

    console.log(server);
    //res.send(server);
    //res.json(server[0]);
  })



tsql.setDefaultConfig(sqlConnection);

// var routes = require('./routes/index');
//var scripts = require('./routes/scripts');
//var main = require('./routes/main');

var app = express();

//view engine setup
app.set('./public/views', path.join(__dirname, './public/views'));
app.set('view engine', 'ejs');

// uncomment after placing your favicon in /public
//app.use(favicon(path.join(__dirname, 'public', 'favicon.ico')));
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

// app.use('/', routes);
//app.use('/scripts/', scripts);
//app.use('/main/', main);

// catch 404 and forward to error handler
app.use(function (req, res, next) {
  var err = new Error('Not Found');
  err.status = 404;
  next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
  app.use(function (err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
      message: err.message,
      error: err
    });
  });
}

// production error handler
// no stacktraces leaked to user
app.use(function (err, req, res, next) {
  res.status(err.status || 500);
  res.render('error', {
    message: err.message,
    error: {}
  });
});


console.log('in root app ');


//var promise = script.getQuestions(connection);
//promise.then(function(connection){

//})


// script.getQuestions(connection);

// script.getQuestions = function(connection,req, res){
//   console.log(connection);
// }

tsql.execute({
  query: "Select * from Question"
}).then(function (results) {
  console.log(results);
}, function (err) {
  console.log("something bad", err);
});

function getData() {
  try {
    let pool = sql.connect(config)
    let result1 = pool.request()
      //.input('input_parameter', sql.Int, value)
      //.query('select * from Question where id = @input_parameter')
      .query('select * from Question')
    console.dir(result1)

    // Stored procedure 

    // let result2 = pool.request()
    //     .input('input_parameter', sql.Int, value)
    //     .output('output_parameter', sql.VarChar(50))
    //     .execute('procedure_name')

    // console.dir(result2)
  } catch (err) {
    // ... error checks 
  }
}



sql.on('error', err => {
  // ... error handler 
  console.log('error');
})


//getData();

function loadEmployees() {
  //4.
  var dbConn = new sql.ConnectionPool(config);
  //5.
  dbConn.connect().then(function () {
    //6.
    var request = new sql.Request(dbConn);
    //7.
    request.query("select * from Question").then(function (recordSet) {
      console.log(recordSet);
      dbConn.close();
    }).catch(function (err) {
      //8.
      console.log(err);
      dbConn.close();
    });
  }).catch(function (err) {
    //9.
    console.log(err);
  });
}
//10.
//loadEmployees();






module.exports = app;
