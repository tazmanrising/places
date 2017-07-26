module.exports = function (db) {

    const sql = require('mssql');
    var schedule = require('node-schedule');
    var config = require('../config.js');
    var sqlConnection = config.database.sbc;

    //SBCswb process
    console.log('start sbcswb');
    schedule.scheduleJob('11 * * * *', function () {
        const poolSbcswb = new sql.ConnectionPool(sqlConnection, err => {
            // ... error checks
            console.log('sbcswb stream');

            const request = new sql.Request(poolSbcswb);
            request.stream = true; // You can set streaming differently for each request
            var query = "Select m1.* from dbo.tblSWBMainLog m1 " +
                "join" +
                " (select SWBMainId, max(loggedAt)as loggedAt from dbo.tblSWBMainLog " +
                " group by SWBMainId ) as m2  on m2.SWBMainId = m1.SWBMainId and m2.loggedAt = m1.loggedAt " +
                "  where Verified in (0,1)   and m1.loggedAt >  dateadd(hh,-1,getdate()) "  ;
            request.query(query);
            request.on('recordset', columns => {
                // Emitted once for each recordset in a query
            });

            request.on('row', row => {
                var requestOD = new sql.Request(poolSbcswb);
                requestOD.input('SWBMainId', sql.Int, row.SWBMainId);
                requestOD.query('select * from dbo.tblSWBTN where SwbMainId = @SWBMainId', (err, result) => {
                    // console.log('order details')
                    // console.log(result.recordset)
                    delete row.logId;  // remove the log info
                    delete row.loggedAt;  // remove the log info
                    row.client = "Sbcswb";
                    row.OrderDetails = result.recordset;
                    db.TPV.update({ SWBMainId: row.SWBMainId }, row, { upsert: true }, (err, data) => {
                        if (err) console.log(err);
                        // console.log(data);
                    });

                });
            });

            request.on('error', err => {
                if(err) console.log(err)
            });

            request.on('done', result => {
                console.log('records written to Mongo');
            });
        });

        sql.on('error', err => {
            if(err) console.log(err)
        });

    });
    
    //SBCwhp process
    console.log('start sbcwhp');
    schedule.scheduleJob('12 * * * *', function () {
        const poolsbcwhp = new sql.ConnectionPool(sqlConnection, err => {
            // ... error checks
            console.log('sbcwhp stream');

            const request = new sql.Request(poolsbcwhp);
            request.stream = true; // You can set streaming differently for each request
            var query = "Select m1.* from dbo.tblWHPMainLog m1 " +
                "join" +
                " (select WHPMainId, max(loggedAt)as loggedAt from dbo.tblWHPMainLog " +
                " group by WHPMainId ) as m2  on m2.WHPMainId = m1.WHPMainId and m2.loggedAt = m1.loggedAt " +
                "  where Verified in (0,1)   and m1.loggedAt >  dateadd(hh,-1,getdate()) "  ;
            request.query(query);
            request.on('recordset', columns => {
                // Emitted once for each recordset in a query
            });

            request.on('row', row => {
                var requestOD = new sql.Request(poolsbcwhp);
                requestOD.input('WHPMainId', sql.Int, row.WHPMainId);
                requestOD.query('select * from dbo.tblWHPTN where WHPMainId = @WHPMainId', (err, result) => {
                    // console.log('order details')
                    // console.log(result.recordset)
                    delete row.logId;  // remove the log info
                    delete row.loggedAt;  // remove the log info
                    row.client = "Sbcwhp";
                    row.OrderDetails = result.recordset;
                    db.TPV.update({ WHPMainId: row.WHPMainId, client: "Sbcwhp" }, row, { upsert: true }, (err, data) => {
                        if (err) console.log(err);
                        // console.log(data);
                    });

                });
            });

            request.on('error', err => {
                // May be emitted multiple times
            });

            request.on('done', result => {
                console.log('records written to Mongo Sbcwhp');
            });
        });

        sql.on('error', err => {
            // ... error handler
        });

    });

};