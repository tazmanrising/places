var sql = require('mssql');

exports.getQuestionDirectives = function (connection, req, res) {
    var request = new sql.Request(connection);
    var query = 'SELECT * from scripts.Directives ' +
        " Order by Id"; 
    console.log(query);
    request.query(query).then(function (resultset) {
        res.json(resultset.recordset)
    }).catch(function (err) {
        console.log('script.directives', err);
        res.json(err)
    })
}
