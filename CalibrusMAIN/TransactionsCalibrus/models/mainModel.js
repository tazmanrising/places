var mongoose = require('mongoose');
var Schema = mongoose.Schema;
var mainSchema = new Schema({
    MainId: String,
    Concern: String 
},{collection: 'server'});

var Server = mongoose.model('server', mainSchema);
module.exports = Server;