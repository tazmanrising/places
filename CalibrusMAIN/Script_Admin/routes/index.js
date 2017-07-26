var express = require('express');
var router = express.Router();

/* GET home page. */
module.exports = function(app){}
router.get('/', function(req, res, next) {
  res.sendfile('index.html', { root: app.get('appPath') });
});

module.exports = router;
