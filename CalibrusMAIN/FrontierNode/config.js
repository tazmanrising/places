var config = {};
var database = {};

config.server = '10.100.60.23';
config.user = 'sa';
config.password = '';
config.database = 'Calibrus';

database.frontier ={}
database.frontier.server = 'tmpsql2'
database.frontier.database = 'frontier'
database.frontier.user = 'calwrite'
database.frontier.password = 'wsql2w'

config.database = database;

module.exports = config;
