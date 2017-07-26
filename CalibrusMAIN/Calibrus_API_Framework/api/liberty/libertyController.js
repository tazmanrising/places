(function () {
    'use strict';
    var sql = require('mssql');


    /* ====== LIBERTY SPECIFIC ======================
    
       Author:  Tom Stickel
       Date:   7/17/2017 
       Notes:
       License:   Calibrus
    */

    exports.getMarketUtility = function (connection, req, res) {
        var request = new sql.Request(connection);
        request.input('Id', sql.Int, req.params.id)
        var query = 'SELECT * FROM [Liberty].[v1].[MarketUtility] ' +
            ' WHERE Active = 1 AND IsElectric = 1 ' +    // IsGas = 1 
            ' AND MarketStateId = @Id';
        request.query(query).then(function (resultset) {
            console.log(resultset.recordset);
            res.json(resultset.recordset);
        }).catch(function (err) {
            console.log('res.json(err)', err);
            res.json(err);
        });

    }

    exports.getMarketProduct = function (connection, req, res) {
        var request = new sql.Request(connection);
        request.input('Id', sql.Int, req.params.id)
        var query = 'SELECT * FROM [Liberty].[v1].[MarketProduct] ' +
            ' WHERE Active = 1 ' +    // IsGas = 1 
            ' AND MarketStateId = @Id ' +
            ' ORDER BY ProductWebForm';
        request.query(query).then(function (resultset) {
            console.log(resultset.recordset);
            res.json(resultset.recordset);
        }).catch(function (err) {
            console.log('res.json(err)', err);
            res.json(err);
        });

    }

    exports.getMarketState = function (connection, req, res) {
        var request = new sql.Request(connection);
        var query = 'SELECT TOP (1000) [MarketStateId] ' +
            ',[State] ,[Active] FROM [Liberty].[v1].[MarketState] ' +
            ' WHERE Active = 1 ORDER BY STATE';
        request.query(query).then(function (resultset) {
            console.log(resultset.recordset);
            res.json(resultset.recordset);
        }).catch(function (err) {
            console.log('res.json(err)', err);
            res.json(err);
        });

    }



    /* ==============  END LIBERTY SPECIFIC ============= */




    exports.getScriptQuestions = function (connection, req, res) {
        var request = new sql.Request(connection);
        var query = 'SELECT * FROM ' +
            ' dbo.vwScriptQuestions vwsq';

        request.query(query).then(function (resultset) {
            console.log(resultset);
            res.json(resultset.recordset);
        }).catch(function (err) {
            console.log('res.json(err)', err);
            res.json(err);
        });

    }

    exports.getQuestionDirectives = function (connection, req, res) {
        var request = new sql.Request(connection);
        request.input('Id', sql.Int, req.params.id)
        var query = 'SELECT q.id AS QuestionId, d.*, qda.* FROM ' +
            ' Scripts.Question q ' +
            ' JOIN Scripts.QuestionDirectiveAssoc qda ON q.Id = qda.QuestionId' +
            ' JOIN Scripts.Directives d on qda.DirectiveId = d.Id ' +
            ' WHERE q.Id = @Id';

        request.query(query).then(function (resultset) {
            console.log(resultset.recordset);
            res.json(resultset.recordset);
        }).catch(function (err) {
            console.log('res.json(err)', err);
            res.json(err);
        });


    }

    exports.getScriptQuestionsOLD = function (connection, req, res) {
        //console.log('conn',connection);

        const poolLiberty = new sql.ConnectionPool(newConfig, err => {
            var obj = "";
            var responseData = [];

            var request = new sql.Request(poolLiberty);
            request.stream = true;
            var query = 'SELECT * FROM ' +
                ' dbo.vwScriptQuestions vwsq';

            request.query(query);
            //console.log(request.query);
            request.on('recordset', columms => {
                // emit once
                //console.log('columns');
            });

            request.on('row', row => {

                //console.log('row:', row);


                var requestOD = new sql.Request(poolLiberty);
                requestOD.input('QuestionId', sql.Int, row.QuestionId);
                var sqlquery = 'SELECT q.id AS QuestionId, d.* FROM ' +
                    ' Scripts.Question q ' +
                    ' JOIN Scripts.QuestionDirectiveAssoc qda ON q.Id = qda.QuestionId' +
                    ' JOIN Scripts.Directives d on qda.DirectiveId = d.Id ' +
                    ' WHERE q.Id = @QuestionId';
                requestOD.query(sqlquery, (err, result) => {
                    row.directives = result.recordset;
                    //console.log('row', row)


                    //console.log(row.directives[0].Tag);
                    obj = row.directives[0].Tag;
                    //obj.push(row.directives);
                    //obj = extend({}, row.directives);
                    responseData.push(row);

                    //console.log(result.recordset);
                    //console.log('directives',row.directives);
                });
                //console.log(row);
                //

            })

            request.on('error', err => {
                // May be emitted multiple times
                console.log('err', err);
            })



            // request.on('done', function (rowCount, more, rows) {
            //     console.log('rowCount',rowCount);
            //     console.log('more',more);
            //     console.log('rows',rows);
            // });

            request.on('done', result => {
                console.log('done x records', result);
                console.log('request', request.query.recordsets);

                console.log(responseData);
                console.log('obj', obj)
                //res.json(vm);
                res.json(responseData);

            })


        })

        sql.on('error', err => {
            console.log('err happened', err);
        })

    }

}());