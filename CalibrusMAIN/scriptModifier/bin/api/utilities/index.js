/**
 * Created by stephenward on 5/11/16.
 */
var express = require('express');
var controller = require('./utilities.controller');
var router = express.Router();


router.get('/:id', controller.getCall);
router.post('/', controller.postCall);
router.post('/update', controller.updateCall);


router.get('/status', controller.getStatus);


module.exports = router;
