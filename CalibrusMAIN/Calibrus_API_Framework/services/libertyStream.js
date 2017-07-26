module.exports = function (db) {

    const sql = require('mssql');
    var schedule = require('node-schedule');
    var config = require('../config.js');
    var sqlConnection = config.database.liberty;

    console.log('start liberty');
    schedule.scheduleJob('22 * * * *', function () {
        const poolLiberty = new sql.ConnectionPool(sqlConnection, err => {
            // ... error checks
            console.log('liberty stream');

            const request = new sql.Request(poolLiberty);
            request.stream = true; // You can set streaming differently for each request
            var query = "Select m1.* from v1.MainLog m1 " +
                "join" +
                " (select mainid, max(loggedAt)as loggedAt from v1.MainLog " +
                " group by mainid ) as m2  on m2.MainId = m1.MainId and m2.loggedAt = m1.loggedAt " +
                "  where Verified in (0,1) and m1.loggedAt >  dateadd(hh,-1,getdate()) "  ;
            request.query(query);
            request.on('recordset', columns => {
                // Emitted once for each recordset in a query
            });

            request.on('row', row => {
                var requestOD = new sql.Request(poolLiberty);
                requestOD.input('mainid', sql.Int, row.MainId);
                requestOD.query('select * from v1.OrderDetail where MainId = @mainid', (err, result) => {
                    // console.log('order details')
                    // console.log(result.recordset)
                    delete row.logId;  // remove the log info
                    delete row.loggedAt;  // remove the log info
                    row.client = "Liberty";
                    row.OrderDetails = result.recordset;
                    db.TPV.update({ MainId: row.MainId, client: "Liberty" }, row, { upsert: true }, (err, data) => {
                        if (err) console.log(err);
                        // console.log(data);
                    });
                });
            });

            request.on('error', err => {
                if(err) console.log(err)
            });

            request.on('done', result => {
                console.log('records written to Mongo Liberty');
            });
        });

        sql.on('error', err => {
            if(err) console.log(err)
        });

    });
};