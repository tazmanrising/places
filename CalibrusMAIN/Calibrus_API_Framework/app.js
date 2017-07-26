var express = require('express');
var path = require('path');
var favicon = require('serve-favicon');
var logger = require('morgan');
var cookieParser = require('cookie-parser');
var bodyParser = require('body-parser');
var cors = require('cors');
var os = require("os");
var config = require('./config.js')

var app = express();
app.use(cors());
//mongo connection

var mongojs = require('mongojs');
var collections = ['TPV']
var db = mongojs(config.mongo, collections)

db.on('error', function (err) {
    console.log('database error', err)
})


db.on('connect', function () {
    console.log(db)
})

// view engine setup
app.set('views', path.join(__dirname, 'views'));
app.set('view engine', 'jade');


// uncomment after placing your favicon in /public
//app.use(favicon(path.join(__dirname, 'public', 'favicon.ico')));
app.use(logger('dev'));
app.use(bodyParser.json());
app.use(bodyParser.urlencoded({ extended: false }));
app.use(cookieParser());
app.use(express.static(path.join(__dirname, 'public')));

require('./routes')(app);


//console.log('os.hostname();',os.hostname());

if (os.hostname() === 'tstickel') {
  console.log('tom machine local');

} else {
    require('./services/sparkStream')(db);
    require('./services/clearviewStream')(db);
    require('./services/libertyStream')(db);
    require('./services/constellationStream')(db);
    require('./services/championStream')(db);
    require('./services/frontierStream')(db);
    require('./services/acsalaskaStream')(db);
    require('./services/bellsouthStream')(db);
    require('./services/centurylinkloaStream')(db);
    require('./services/centurytelStream')(db);
    require('./services/coxStream')(db);
    require('./services/gciStream')(db);
    require('./services/merrymaidsStream')(db)
    require('./services/lesliespoolStream')(db);
    require('./services/qwesttpvStream')(db);
    require('./services/sbcStream')(db);
    require('./services/attStream')(db);
    require('./services/chubbStream')(db);
    require('./services/hagertyStream')(db);
    require('./services/miconnectionStream')(db);
    require('./services/societyStream')(db);
    require('./services/texpoStream')(db);
}



// catch 404 and forward to error handler
app.use(function(req, res, next) {
  var err = new Error('Not Found');
  err.status = 404;
  next(err);
});

// error handlers

// development error handler
// will print stacktrace
if (app.get('env') === 'development') {
  app.use(function(err, req, res, next) {
    res.status(err.status || 500);
    res.render('error', {
      message: err.message,
      error: err
    });
  });
}

// production error handler
// no stacktraces leaked to user
app.use(function(err, req, res, next) {
  res.status(err.status || 500);
  res.render('error', {
    message: err.message,
    error: {}
  });
});


module.exports = app;
