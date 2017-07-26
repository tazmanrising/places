var config = {};

var database = {};

config.server = '10.100.60.23';
config.user = 'sa';
config.password = '';
config.database = 'Calibrus';


database.liberty ={}
database.liberty.server = 'tmpsql2'
database.liberty.database = 'liberty'
database.liberty.user = 'calwrite'
database.liberty.password = 'wsql2w'

config.database = database;

module.exports = config;
