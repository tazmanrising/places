var express = require('express');
var router = express.Router();
//var Server = require('../models/serverModel');
var bodyParser = require('body-parser');

//var config =  require('../config.js') ;
//console.log(config)

//var mongoose = require('mongoose');
//var config = require('../config');
//mongoose.connect(config.getDbConnectionString());

//mongodb://' + configValues.uname + ':' + configValues.pwd + '@10.100.40.204:27017/CalibrusServers

//mongoose.connect('mongodb://cal:cal@10.100.40.204:27017/CalibrusServers');



/* GET users listing. */
router.get('/', function (req, res) {
    // var request = new sql.Request(connection);
    // request.execute('spScriptEditor', function (err, recordsets) {
    //     res.json(recordsets[0]);
    // })
    console.log('test');
    
    Server.find({},
        function(err, server) {
            if(err) throw err;

            console.log(server);
            res.send(server);
            //res.json(server[0]);
        })
    //res.json({test:'test'});
});

router.post('/save', function(req, res){
        console.log('save', req.body);
        if(req.body.id){
            // has id update
            //Todos.findByIdAndUpdate(req.body.id, { todo: req.body.todo, isDone: req.body.isDone, hasAttachment: req.body.hasAttachment  },
            Server.findByIdAndUpdate(req.body.id, { Type: req.body.Type, Name: req.body.Name, OS: req.body.OS },
            function(err, todo){
                if(err) throw err;

                res.send('Success');
            })
        }else{
            // new  do insert
            var newTodo = Todos({
                username: 'test',
                todo: req.body.todo,
                isDone: req.body.isDone,
                hasAttachment: req.body.hasAttachment
            });
            newTodo.save(function(err){
                if(err) throw err;

                res.send('Success');
            });
        }


});






module.exports = router;