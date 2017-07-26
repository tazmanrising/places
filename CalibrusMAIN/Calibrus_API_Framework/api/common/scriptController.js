/**
 * Created by sward on 5/5/2017.
 */
// scripting API's  CRUD operations
var sql = require('mssql');
var config = require('../../config.js');
var sqlConfig = config.database.liberty; //TODO:   change 

exports.createQuestion = function (connection, req, res) {

    //console.log('exports.createQuestion', req.body);

    var request = new sql.Request(connection);
    var query = "insert into Scripts.Question( Name, Description, Verbiage, VerbiageSpanish, ParentId, ParentValue, Active )" +
        " VALUES(@Name,@Description, @Verbiage, @VerbiageSpanish, @ParentId, @ParentValue, @Active) ; " +
        "Select scope_identity() as QuestionId"

    console.log('create question insert - req.body', req.body);
    
    request.input('Name', sql.VarChar, req.body.Name);
    request.input('Description', sql.VarChar, req.body.Description);
    request.input('Verbiage', sql.VarChar, req.body.Verbiage);
    request.input('VerbiageSpanish', sql.VarChar, req.body.VerbiageSpanish);
    request.input('ParentId', sql.Int, req.body.ParentId);
    request.input('ParentValue', sql.VarChar, req.body.ParentValue);
    request.input('Active', sql.Bit, req.body.Active);
    //request.input('LabelEnglish', sql.VarChar, req.body.LabelEnglish);
    //request.input('LabelSpanish', sql.VarChar, req.body.LabelSpanish);

    request.query(query).then(function (resultset) {
        //console.log(resultset)
        //console.log('resultset insert', resultset.recordsets[0][0].QuestionId); //.QuestionId);

        var direct = {};
        direct.QuestionId = resultset.recordsets[0][0].QuestionId; //req.body.Id;
        direct.SortOrder = req.body.SortOrder;
        direct.directive = req.body.directive;
        direct.data = req.body.data;

        manageDirectives(connection, direct);

        res.json(resultset);

    }).catch(function (err) {
        console.log('ins question', err);
        res.json(err)
    })
}

exports.ModifyQuestion = function (connection, req, res) {
    var request = new sql.Request(connection);
    request.input('Id', sql.Int, req.body.Id);
    request.input('Name', sql.VarChar, req.body.Name);
    request.input('Description', sql.VarChar, req.body.Description);
    request.input('Verbiage', sql.VarChar, req.body.Verbiage);
    request.input('VerbiageSpanish', sql.VarChar, req.body.VerbiageSpanish);
    request.input('ParentId', sql.Int, req.body.ParentId);
    request.input('ParentValue', sql.VarChar, req.body.ParentValue);
    request.input('Active', sql.Bit, req.body.Active);

    var query = "Update Scripts.Question " +
        " Set Name = @Name " +
        ", Description = @Description " +
        ", Verbiage = @Verbiage " +
        ", VerbiageSpanish = @VerbiageSpanish " +
        ", ParentId = @ParentId " +
        ", ParentValue = @ParentValue " +
        ", Active = @Active " +
        " Where  id = @Id "


    // Modify Directives Associations
    console.log('update to question before managedirective');

    var direct = {};
    direct.QuestionId = req.body.Id;
    direct.SortOrder = req.body.SortOrder;
    direct.directive = req.body.directive;
    direct.data = req.body.data;

    manageDirectives(connection, direct);

    request.query(query).then(function (resultset) {
        res.json(resultset)
    }).catch(function (err) {
        res.json('error on update of question', err)
    });



}

var manageDirectives = function (connection, req) {


    var request = new sql.Request(connection);
    var query = 'DELETE FROM Scripts.QuestionDirectiveAssoc where QuestionId = ' + req.QuestionId;

    console.log('about to delete', query);
    request.query(query).then(function (r) {
        console.log('deleted all from questionid: ' + req.QuestionId);
    }).catch(function (err) {
        console.log('err node', err);
        console.log(err);
    });

    console.log('about to loop ', req);
    var i = 1;
    for (var d in req) {
        if (d === "directive") {
            for (var prop in req[d]) {
                //console.log(prop + ":" + req[d][prop]);
                if (req[d][prop] == true) {
                    var query2 = "INSERT INTO Scripts.QuestionDirectiveAssoc " +
                        '(QuestionId, DirectiveId, SortOrder, data) ' +
                         " VALUES(" + req.QuestionId + "," + prop + "," + req.SortOrder[i] + ",'" + req.data[i] + "');"
                    //console.log('i='+ i + ', req=' +req);
                    console.log('req sortorder',req.SortOrder[i]);
                    console.log('query2', query2);
                    request.query(query2).then(function (result) {
                        //console.log('insert id: ' + prop);
                        
                    }).catch(function (err) {
                        console.log('err node', err);
                        //console.log('req', req);
                        console.log('prop', prop);
                        console.log('question dir assoc query2', query2)
                        //res.json(err)
                    });
                }
                i++;
            }
        }
    }

}


exports.getDirectiveAssoc = function (connection, req, res) {
    console.log('in');
    var request = new sql.Request(connection);
    request.input('QuestionId', sql.VarChar, req.params.questionid);
    console.log('req.params.QuestionId', req.params.questionid);
    var query = 'select * from Scripts.QuestionDirectiveAssoc where QuestionId = @QuestionId ';
    console.log('query', query);
    request.query(query).then(function (resultset) {
        console.log('resultset dir assoc', resultset);
        res.json(resultset.recordset);
    }).catch(function (err) {
        res.json(err)
    });
}


exports.getQuestions = function (connection, req, res) {
    var request = new sql.Request(connection);
    var query = 'select * from Scripts.Question'
    request.query(query).then(function (resultset) {
        res.json(resultset.recordset);
    }).catch(function (err) {
        res.json(err)
    });
}

// return script questions
exports.getScriptQuestions = function (connection, req, res) {

    var request = new sql.Request(connection);
    request.input('StateCode', sql.VarChar, req.params.statecode);
    request.input('SalesChannelId', sql.Int, req.params.saleschannelid);
    var query = 'SELECT * from vwScriptQuestions ' +
        "WHERE (stateCode = @StateCode or stateCode = 'AA' )" +
        ' And SalesChannelId = @SalesChannelId'
    console.log(query);
    request.query(query).then(function (resultset) {
        res.json(resultset.recordset)
    }).catch(function (err) {
        console.log('vwScriptQuestions', err);
        res.json(err)
    })

}

exports.createScriptQuestion = function (connection, req, res) {

    var request = new sql.Request(connection);
    request.input('QtypeId', sql.Int, req.body.QtypeId);
    request.input('StateCode', sql.VarChar, req.body.StateCode);
    request.input('SalesChannelId', sql.Int, req.body.SalesChannelId);
    request.input('QuestionId', sql.Int, req.body.QuestionId);
    request.input('ScriptOrder', sql.Int, req.body.ScriptOrder);
    request.input('Active', sql.Bit, req.body.Active);

    var query = "insert into Scripts.ScriptQuestions( QtypeId, StateCode, SalesChannelId, QuestionId,ScriptOrder,Active)" +
        " VALUES(@QtypeId, @StateCode, @SalesChannelId, @QuestionId,@ScriptOrder,@Active) ; " +
        "Select * from vwScriptQuestions where ScriptId = SCOPE_IDENTITY()"

    request.query(query).then(function (resultset) {
        res.json(resultset)
    }).catch(function (err) {
        res.json(err)
    });
}

exports.ModifyScriptQuestion = function (connection, req, res) {
    var request = new sql.Request(connection);
    request.input('QtypeId', sql.Int, req.body.QtypeId);
    request.input('StateCode', sql.VarChar, req.body.StateCode);
    request.input('SalesChannelId', sql.Int, req.body.SalesChannelId);
    request.input('QuestionId', sql.Int, req.body.QuestionId);
    request.input('ScriptId', sql.Int, req.body.ScriptId);
    request.input('ScriptOrder', sql.Int, req.body.ScriptOrder);
    request.input('Active', sql.Bit, req.body.Active);


    var query = "Update Scripts.ScriptQuestions " +
        " Set QtypeId = @QtypeId " +
        ", StateCode = @StateCode " +
        ", SalesChannelId = @SalesChannelId " +
        ", QuestionId = @QuestionId " +
        ", ScriptOrder = @ScriptOrder " +
        ", Active = @Active " +
        " Where ScriptId = @ScriptId "

    request.query(query).then(function (resultset) {
        res.json(resultset)
    }).catch(function (err) {
        res.json(err)
    });

}

// exports.getScripts = function (state, market, req, res) {
//     var script = []
//     var scriptid = 0;
//     (async function () {
//         try {
//             let pool = await sql.connect(sqlConfig)
//             var query = "SELECT sq.* , d.*, sqa.data, sqa.SortOrder from [dbo].[vwScriptQuestions] sq " +
//                 "join [Scripts].[QuestionDirectiveAssoc] sqa on sqa.QuestionId = sq.QuestionId " +
//                 "join [Scripts].[Directives] d on sqa.DirectiveId = d.Id " +
//                 "where (StateCode = @statecode or StateCode = 'AA')  and SalesChannelId = @market " +
//                 "order by scriptid, qtype, ScriptOrder"

//             //console.log('query',query);

//             let result1 = await pool.request()
//                 .input('statecode', sql.VarChar, req.params.state)
//                 .input('market', sql.Int, req.params.market)
//                 .query(query)

//             //console.dir(result1)
//             result1.recordset.forEach(function (x) {
//                 console.log('x', x);

//                 if (scriptid != x.ScriptId) {
//                     x.Directives = []
//                     x.Directives.push({ 'Name': x.Name, 'Tag': x.Tag, 'Data': x.data, 'SortOrder': x.SortOrder })
//                     i = script.push(x)
//                     scriptid = x.ScriptId
//                 } else {
//                     script[i - 1].Directives.push({ 'Name': x.Name, 'Tag': x.Tag, 'Data': x.data, 'SortOrder': x.SortOrder })
//                 }

//             })

//             res.json(script)

//         } catch (err) {
//             // ... error checks
//             if (err) console.log(err)
//         }

//         sql.close()

//     })()

// }





