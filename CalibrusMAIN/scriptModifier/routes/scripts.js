var express = require('express');
var router = express.Router();
var sql = require('mssql');

var config =  require('../config.js') ;
console.log(config)

var connection = new sql.Connection(config, function (err) {
    console.log(err)

})


/* GET users listing. */
router.get('/', function (req, res, next) {
    var request = new sql.Request(connection);
    request.execute('spScriptEditor', function (err, recordsets) {
        res.json(recordsets[0]);
    })
});

router.get('/:db', function (req, res, next) {
    var request = new sql.Request(connection);
    request.input('db', sql.VarChar(50), req.params.db);
    //console.log(request);
    request.execute('spScriptEditor ', function (err, recordsets) {
        console.log(err)
        res.json(recordsets);
    })
});

router.get('/:db/:table', function (req, res, next) {
    var request = new sql.Request(connection);
    request.input('db', sql.VarChar(50), req.params.db);
    request.input('table', sql.VarChar(50), req.params.table);
    //console.log(request);
    request.execute('spScriptEditor ', function (err, recordsets) {
        console.log(err)
        res.json(recordsets);
    })
});


router.get('/:db/:table', function (req, res, next) {
    var request = new sql.Request(connection);
    request.input('db', sql.VarChar(50), req.params.db);
    request.input('table', sql.VarChar(50), req.params.table);
    //console.log(request);
    request.execute('spScriptEditor ', function (err, recordsets) {
        console.log(err)
        res.json(recordsets);
    })
});

router.post('/saveOld', function (req, res) {
    console.log(req.body);
    var fields = 0;
    var SQL = 'Update ' + req.body.db.name + req.body.table + ' SET ';
    var SET = '';
    var WHERE = ' WHERE ScriptId = ' + req.body.ScriptId;

    if (req.body.Verbiage) {
        if (fields == 0) {
            SET += 'Verbiage = ' + "'" + req.body.Verbiage + "'";
        } else {
            SET += ', Verbiage = ' + "'" + req.body.Verbiage + "'";
        }
        fields++
        //  console.log(SET)
    }
    if (req.body.VerbiageSpanish) {
        if (fields == 0) {
            SET += 'VerbiageSpanish = ' + "'" + req.body.VerbiageSpanish + "'";
        } else {
            SET += ', VerbiageSpanish = ' + "'" + req.body.VerbiageSpanish + "'";
        }
        fields++
        // console.log(SET)
    }

    if (req.body.ScriptOrder) {
        if (fields == 0) {
            SET += 'ScriptOrder = ' + req.body.ScriptOrder;
        } else {
            SET += ', ScriptOrder = ' + req.body.ScriptOrder;
        }
        fields++
        //  console.log(SET)
    }

    if (req.body.Active) {
        if (req.body.Active == true) {
            req.body.Active = 1
        } else {
            req.body.Active = 0
        }

        if (fields == 0) {
            SET += 'Active = ' + req.body.Active;
        } else {
            SET += ', Active = ' + req.body.Active;
        }
        fields++
        // console.log(SET)
    }

    if (req.body.Condition) {
        var re = /'/g;
        req.body.Condition = req.body.Condition.replace(re, "''");
        if (fields == 0) {
            SET += 'Condition = ' + "'" + req.body.Condition + "'";
        } else {
            SET += ', Condition = ' + "'" + req.body.Condition + "'";
        }
        fields++
        // console.log(SET)
    }


    var query = SQL + SET + WHERE;
    console.log(query);
    var request = new sql.Request(connection);
    // request.verbose =true ;
    request.query(query).then(function (recordset) {
        //console.log(request.rowsAffected)
        res.json({updated: request.rowsAffected})
    }).catch(function (err) {
        console.log(err)
        res.json(err)
    })


})

router.post('/save', function (req, res) {

    var request = new sql.Request(connection);
    request.input('verbiage', sql.VarChar, req.body.Verbiage);
    request.input('verbiageSpanish', sql.VarChar, req.body.VerbiageSpanish);
    request.input('noverbiage', sql.VarChar, req.body.NoVerbiage);
    request.input('noverbiageSpanish', sql.VarChar, req.body.NoVerbiageSpanish);
    request.input('scriptorder', sql.Int, req.body.ScriptOrder);
    request.input('active', sql.Bit, req.body.Active);
    request.input('loop', sql.VarChar, req.body.Loop);
    request.input('yesno', sql.Bit, req.body.YesNo);
    request.input('condition', sql.VarChar, req.body.Condition);
    request.input('concerncode', sql.VarChar, req.body.NoConcernCode);


    var SQL = 'Update ' + req.body.db.name + req.body.table + ' SET ';
    var SET = '';
    var WHERE = ' WHERE ScriptId = ' + req.body.ScriptId;

    SET += ' Verbiage = @verbiage';
    SET += ', VerbiageSpanish = @verbiageSpanish';
    SET += ', NoVerbiage = @noverbiage';
    SET += ', NoVerbiageSpanish = @noverbiageSpanish';
    SET += ', ScriptOrder = @scriptorder';
    SET += ', Active = @active';
    SET += ', YesNo = @yesno';
    SET += ', Condition = @condition';
    SET += ', NoConcernCode = @concerncode';

    if(req.body.Loop)
    {
        SET += ',Loop = @loop' ;
    }
    var query = SQL + SET + WHERE;
    console.log(query);
     request.verbose =true ;
    request.query(query).then(function (recordset) {
     //   console.log(request.RowsAffected);
        res.json({updated: request.rowsAffected})
    }).catch(function (err) {
      //  console.log(request)
        res.json(err)
    })
});

module.exports = router;
