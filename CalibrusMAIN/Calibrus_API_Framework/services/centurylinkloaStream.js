module.exports = function (db) {

    const sql = require('mssql');
    var schedule = require('node-schedule');
    var config = require('../config.js');
    var sqlConnection = config.database.centurylinkloa;


    console.log('start centurylinkloa');
    schedule.scheduleJob('19 * * * *', function () {
        const poolCenturylinkloa = new sql.ConnectionPool(sqlConnection, err => {
            // ... error checks
            console.log('centurylinkloa stream');

            const request = new sql.Request(poolCenturylinkloa);
            request.stream = true; // You can set streaming differently for each request
            var query = "Select m1.* from dbo.tblMainLog m1 " +
                "join" +
                " (select mainid, max(loggedAt)as loggedAt from dbo.tblMainLog " +
                " group by mainid ) as m2  on m2.MainId = m1.MainId and m2.loggedAt = m1.loggedAt " +
                "  where Verified in (0,1) and m1.loggedAt >  dateadd(hh,-1,getdate()) "  ;
            request.query(query);
            request.on('recordset', columns => {
                // Emitted once for each recordset in a query
            });

            request.on('row', row => {
                var requestOD = new sql.Request(poolCenturylinkloa);
                requestOD.input('mainid', sql.Int, row.MainId);
                requestOD.query('select * from dbo.tblTn where MainId = @mainid', (err, result) => {
                    if(err) {
                        console.log('ordnode er details')
                        console.log(err)
                    }
                    delete row.logId;  // remove the log info
                    delete row.loggedAt;  // remove the log info
                    row.client = "Centurylinkloa";
                    row.OrderDetails = result.recordset;
                    db.TPV.update({ MainId: row.MainId, client: "Centurylinkloa" }, row, { upsert: true }, (err, data) => {
                        if (err) console.log(err);
                        // console.log(data);
                    });

                });
            });

            request.on('error', err => {
                if(err) console.log(err)
            });

            request.on('done', result => {
                console.log('records written to Mongo - Centurylinkloa');
            });
        });

        sql.on('error', err => {
            if(err) console.log(err)
        });

    });
};