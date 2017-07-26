/**
 * Created by stephenward on 5/11/16.
 */
var _ = require('lodash');
var async = require('async');
var Calls = require('../../models/call.model.js');
var config = require('../../config/environment');

var algoliasearch = require('algoliasearch');
var client = algoliasearch(config.algolia.applicationId, config.algolia.apiKey);

exports.getStatus = function (req, res) {

    res.json();
}

exports.postCall = function(req,res){
    Calls.create(req.body, function(err,data){
        if(err) return res.json(err);
        res.json(data);
    })
}

exports.updateCall = function(req,res){

    Calls.update({_id:req.body._id},req.body,function(err,data){
        if(err) console.log(err);
        res.json(data);
    })
}


exports.getCall = function(req,res){
    var id = req.params.id ;
    Calls.find({_id: id}, function(err,data){
        if(err) res.json(err);
        res.json(data);
        }
    )
}

