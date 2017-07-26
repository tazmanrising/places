var express = require('express');
var router = express.Router();


module.exports = function(app) {

     app.route('/*')
        .get(function(req, res) {
            res.sendfile(app.get('appPath') + '/index.html');
        });
};