module.exports = function (db) {

    const sql = require('mssql');
    var schedule = require('node-schedule');
    var config = require('../config.js');
    var sqlConnection = config.database.lesliespool;


    console.log('start lesliespool');
    schedule.scheduleJob('14 * * * *', function () {
        const poolLesliespool = new sql.ConnectionPool(sqlConnection, err => {
            // ... error checks
            console.log('lesliespool stream');

            const request = new sql.Request(poolLesliespool);
            request.stream = true; // You can set streaming differently for each request
            var query = "Select m1.* from dbo.tblMainLog  m1 " +
                "join" +
                " (select mainid, max(loggedAt)as loggedAt from dbo.tblMainLog  " +
                " group by mainid ) as m2  on m2.MainId = m1.MainId and m2.loggedAt = m1.loggedAt " +
                "  where  m1.loggedAt >  dateadd(hh,-1,getdate()) ";
            request.query(query);
            request.on('recordset', columns => {
                // Emitted once for each recordset in a query
            });

            request.on('row', row => {

                delete row.logId;  // remove the log info
                delete row.loggedAt;  // remove the log info
                row.client = "Lesliespool";
                
                db.TPV.update({ MainId: row.MainId, client: "Lesliespool" }, row, { upsert: true }, (err, data) => {
                    if (err) console.log(err);
                    // console.log(data);
                });
            });

            request.on('error', err => {
                if(err) console.log(err)
            });

            request.on('done', result => {
                console.log('records written to Mongo Lesliespool');
            });
        });

        sql.on('error', err => {
            if(err) console.log(err)
        });

    });
};