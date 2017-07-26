(function () {
    'use strict';
    var sql = require('mssql');


    exports.getCalibrusAppLog = function (connection, req, res) {
        var request = new sql.Request(connection);
        var query = 'SELECT * FROM ' +
            ' dbo.tblApplication';

        request.query(query).then(function (resultset) {
            console.log(resultset);
            res.json(resultset.recordset);
        }).catch(function (err) {
            console.log('res.json(err)', err);
            res.json(err);
        });
    }



}());