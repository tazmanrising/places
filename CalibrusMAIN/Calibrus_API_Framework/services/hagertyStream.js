module.exports = function (db) {

    const sql = require('mssql');
    var schedule = require('node-schedule');
    var config = require('../config.js');
    var sqlConnection = config.database.hagerty;
    console.log('start hagerty');
    schedule.scheduleJob('6 * * * *', function () {
        const poolhagerty = new sql.ConnectionPool(sqlConnection, err => {
            // ... error checks
            console.log('hagerty stream');

            const request = new sql.Request(poolhagerty);
            request.stream = true; // You can set streaming differently for each request
            var query = "Select m1.* from dbo.tblVSMainLog m1 " +
                "join" +
                " (select VSMainId, max(loggedAt)as loggedAt from dbo.tblVSMainLog " +
                " group by VSMainId ) as m2  on m2.VSMainId = m1.VSMainId and m2.loggedAt = m1.loggedAt " +
                "  where m1.loggedAt >  dateadd(hh,-1,getdate()) ";
            request.query(query);
            request.on('recordset', columns => {
                // Emitted once for each recordset in a query
            });

            request.on('row', row => {
                var requestOD = new sql.Request(poolhagerty);
                requestOD.input('VSMainId', sql.Int, row.VSMainId);
                requestOD.query('select * from dbo.tblVSMainLog where VSMainId = @VSMainId', (err, result) => {
                    // console.log('order details')
                    // console.log(result.recordset)
                    delete row.logId;  // remove the log info
                    delete row.loggedAt;  // remove the log info
                    row.client = "Hagerty";
                    row.OrderDetails = result.recordset;
                    db.TPV.update({ VSMainId: row.VSMainId, client: "Hagerty" }, row, { upsert: true }, (err, data) => {
                        if (err) console.log(err);
                        // console.log(data);
                    });

                });
            });

            request.on('error', err => {
                if(err) console.log(err)
            });

            request.on('done', result => {
                console.log('records written to Mongo Hagerty');
            });
        });

        sql.on('error', err => {
            if(err) console.log(err)
        });

    });
};