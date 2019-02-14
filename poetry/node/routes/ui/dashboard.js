var express = require('express');
var http = require('http');
var path = require('path');
var cors = require('cors');
var fs = require('fs');
var os = require("os");
var async = require("async");
var request = require('request');
var NodeCache = require("node-cache");
var Excel = require("exceljs");
var formidable = require('formidable');
var Promise = require('bluebird');
var _ = require('lodash');


//User Specific

var config = require('../bl/appsetting');
var excel = require(config.get() + "/routes/bl/excel.js");
var logger = require(config.get() + "/routes/bl/logger.js");
var ds = require(config.get() + "/routes/bl/dataset.js");

var url = require('url');
var session = require('express-session');
var dbPath = path.resolve(config.getDataRoot().log + 'database.db')
var sqlite3 = require('sqlite3').verbose();
var sqlite = require('sqlite-sync');
var router = express.Router();

var multer = require('multer');
var upload = multer({
  dest: config.getDataRoot().db
});


var serverdb = config.get() + "/db/server.json"
var uploadPath = config.getDataRoot().upload;

var mylogger = function () {
  var self = this;
  self.logs = [];
  self.log = function (obj) {
    var line = "";
    if (typeof obj == "object") {
      try {
        line = JSON.stringify(obj);
      } catch (e) {
      }
    } else {
      line = obj;
    }
    console.log(line);
    self.logs.push({
      timestamp: new Date(),
      line: line
    });
  };

  self.clear = function () {
    self.logs = [];
  };
};

var memlogger = new mylogger();


/* GET home page. */

function checkAuth(req, res, next) {
    if (!req.session.login) {
      //res.send('You are not authorized to view this page');
      res.redirect("/unauthorised");
    } else {
      next();
    }
  }
//ui
router.get('/', function(req, res, next) {
    var cookie = req.cookies.squad_sid;
    if(!cookie)
    {
           var randomNumber=Math.random().toString();
           randomNumber= "s_" + randomNumber.substring(2,randomNumber.length);
           res.cookie(config.reader.cookies.session, randomNumber, 
           { 
                maxAge: 5 * 365 * 24 * 60 * 60 * 1000 , // 5 Years, 
                httpOnly: true 
            });
           res.redirect("/schedule/");
    }
    res.redirect("/schedule/dash");
});


router.get('/unauthorised', function(req, res, next) {
    var appModel =config.model();
    appModel.message = "Dashboard";
    appModel.sid = cookie;
    appModel.envs = [];
    res.render('unauthorised',appModel);
});

router.get('/start', function(req, res, next) {

    var cookie = req.cookies.squad_sid;
    if(!cookie)
    {
        res.redirect("/schedule/");
    }
    var appModel =config.model();
    appModel.message = "Dashboard";
    appModel.sid = cookie;
    appModel.envs = [];

    logger.info("Hits:HomePage comming from:" + req.connection.remoteAddress);
    res.render('schedule_start',appModel);
});


router.get('/text', function(req, res, next) {

    var cookie = req.cookies.squad_sid;
    if(!cookie)
    {
        res.redirect("/schedule/");
    }
    var appModel =config.model();
    appModel.message = "Dashboard";
    appModel.sid = cookie;
    appModel.envs = [];

    logger.info("Hits:HomePage comming from:" + req.connection.remoteAddress);
    res.render('schedule_text',appModel);
});

router.get('/dash', function(req, res, next) {

    var cookie = req.cookies.squad_sid;
    if(!cookie)
    {
        res.redirect("/schedule/");
    }
    var appModel =config.model();
    appModel.message = "Dashboard";
    appModel.sid = cookie;
    appModel.envs = [];

    logger.info("Hits:HomePage comming from:" + req.connection.remoteAddress);
    res.render('schedule_dash',appModel);
});


router.get('/test', function(req, res, next) {
    var appModel =config.model();
    var query = req.query;
    var cookie = req.cookies.squad_sid;
    if(!cookie)
    {
        res.redirect("/schedule/");
    }

    appModel.message = "Dashboard";
    appModel.sid = cookie;
    appModel.envs = [];
    appModel.pkid = query.pkid;
    appModel.app = query.app;
    appModel.i = 1;
    appModel.s = query.s==null ? 1:query.s;
    appModel.p = query.p == null ? 1: query.p;

    res.render('test',appModel);
});

module.exports = router;
