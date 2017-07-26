module.exports = function (db) {

    const sql = require('mssql');
    var schedule = require('node-schedule');
    var config = require('../config.js');
    var sqlConnection = config.database.cox;



    console.log('start cox');
    schedule.scheduleJob('20 * * * *', function () {
        const poolCox = new sql.ConnectionPool(sqlConnection, err => {
            // ... error checks
            console.log('cox stream');

            const request = new sql.Request(poolCox);
            request.stream = true; // You can set streaming differently for each request
            var query = "Select m1.* from dbo.CoxMainLog m1 " +
                "join" +
                " (select cmkeyid, max(loggedAt)as loggedAt from dbo.CoxMainLog " +
                " group by cmkeyid ) as m2  on m2.cmkeyid = m1.cmkeyid and m2.loggedAt = m1.loggedAt " +
                "  where Verified in (0,1) and m1.loggedAt  >  dateadd(hh,-1,getdate())";
            request.query(query);
            request.on('recordset', columns => {
                // Emitted once for each recordset in a query
            });

            request.on('row', row => {
                var requestOD = new sql.Request(poolCox);
                requestOD.input('cmkeyid', sql.Int, row.CMKeyId);
                requestOD.query('select * from dbo.CoxWtn where cmkeyid = @cmkeyid', (err, result) => {
                    if(err) console.log(err, 'cox')
                    // console.log(result.recordset)
                    delete row.logId;  // remove the log info
                    delete row.loggedAt;  // remove the log info
                    row.client = "Cox";
                    if(result.recordset){
                        row.OrderDetails = result.recordset;
                    }

                    db.TPV.update({ CMKeyId: row.CMKeyId, client: "Cox" }, row, { upsert: true }, (err, data) => {
                        if (err) console.log(err);
                        // console.log(data);
                    });

                });
            });

            request.on('error', err => {
                if(err) console.log(err)
            });

            request.on('done', result => {
                console.log('records written to Mongo Cox');
            });
        });

        sql.on('error', err => {
            if(err) console.log(err)
        });

    });
};