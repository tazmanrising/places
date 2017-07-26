var sql = require('mssql');

exports.getQuestions = function(connection, req,res){
     console.log(connection);
    var request = new sql.Request(connection);
   
    var query = 'select * from Question'
    request.query(query).then(function(resultset){
        res.json(resultset.recordset);
    }).catch(function(err){
        console.log(err);
        //res.json(err)
    })
}