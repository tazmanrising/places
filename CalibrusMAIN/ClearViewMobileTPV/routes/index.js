var express = require('express');
var router = express.Router();

/* GET home page. */
router.get('/', function(req, res, next) {
  res.render('index', { title: 'Express' });
});

router.get('/consumer', function(req, res, next) {
  res.render('../consumer/index', { title: 'Clearview TPV' });
});


module.exports = router;
