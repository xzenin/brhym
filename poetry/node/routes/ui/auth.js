require('rootpath')();

var express = require('express');
var os = require('os');
var url = require('url');
var session = require('express-session');
var path = require('path');
var dbPath = path.resolve(__dirname, '../../db/database.db')
var sqlite3 = require('sqlite3').verbose();

//User Specific
var config = require('../bl/appsetting');
var excel = require(config.get() + "/routes/bl/excel.js");
var logger = require(config.get() + "/routes/bl/logger.js");


var router = express.Router();


/* GET users listing. */

router.get('/', function(req, res, next) {
  var appModel = db.model(req,res);

  var model = {
    title: appModel.title, 
    desc:appModel.desc,
    message:"Login"
    };
res.render('auth', model);
});


router.get('/db', function(req, res, next) {

  var appModel = db.model(req,res);

  var model = {
    title: appModel.title, 
    desc:appModel.desc,
    message:"Login",
    rows:[]
    };
    
    console.log('opening..' + dbPath);
    var sqldb = new sqlite3.Database(dbPath, function(err){
      if(err)
      {
      console.log("Error in SQLLITE3 database opening:");
      console.log(err);
      }
    });
    sqldb.serialize( function(){

    sqldb.all("select name from sqlite_master where type='table'", function(err, rows) {
      model.rows = rows;
      model.err = err;
      if(err)
      {

        console.log(err);
      }
      else
      {
      console.log("Total rows : " + model.rows.length);
      }
      console.log(err);
      console.log(rows);
      res.render('sqlite_db', model);
    });
  
  });

    sqldb.close(function(err){
      console.log("Error in closing the db.");
    });

});

router.post('/db', function(req, res, next) {
  var appModel = db.model(req,res);
  var token = req.body.token;
  var model = {
    title: appModel.title, 
    desc:appModel.desc,
    message:"Login",
    rows:[]
    };
    
    console.log('opening..' + dbPath);
    var sqldb = new sqlite3.Database(dbPath, function(err){
      console.log("Error in SQLLITE3 database opening:");
      console.log(err);
    });
 
    sqldb.serialize( function(){

    sqldb.run("CREATE TABLE AppUsers ( id text primary key, name text) ", function(){ 
      console.log("AppUsers created.");
    });
    sqldb.run("CREATE TABLE AppRoles (id text primary key, name text) ", function(){ 
      console.log("AppRoles created.");
    });
  
    sqldb.run("CREATE TABLE UserInRoles (user text , role text) ", function(){ 
      console.log("UserInRoles created.");
    });
    
    sqldb.run("CREATE TABLE RoleInModules (role text, module text) ", function(){ 
      console.log("RoleInModules created.");
    });   

    sqldb.run("CREATE TABLE AppStatements ( id text primary key, displayname text, sql text) ", function(){ 
      console.log("AppStatements created.");
    });  

    
    sqldb.run("CREATE TABLE DesignComments ( id text primary key, comment text) ", function(){ 
      console.log("AppStatements created.");
    });  
    
    res.redirect("/auth/db");

   });
    sqldb.close(function(err){
      console.log("Error in closing the db.");
    });  

});

router.get('/users', function(req, res, next) {
  var appModel = db.model(req,res);

  var model = {
    title: appModel.title, 
    desc:appModel.desc,
    message:"Login"
    };

    res.render('auth_users', model);

});

router.get('/login', function(req, res, next) {
  var appModel = db.model(req,res);
  appModel.message = "Dashboard";
  //appModel.sid = cookie;
  appModel.envs = [];
  res.render('auth_login', appModel);
});

router.post('/login', function(req, res, next) {
  var appModel = db.model(req,res);
  var server = req.body.server;
  var port = req.body.port;
  var username = req.body.username;
  var password = req.body.password;

  var fm = {
    db:"LISA",
    server:req.body.server,
    port :req.body.port,
    username : req.body.username,
    password : req.body.password  
  };

  var model = {
    title: appModel.title, 
    desc:appModel.desc,
    message:"",
    error: null,
    fm:fm
    };

    if(username == "db2admin")
    {
        req.session.login = model;
        res.redirect("/ui");
    }
    else
    {
      model.error = "Invalid login";
      model.message = "Invalid login";
    } 
  res.render('setting', model);
});

router.get('/logout', function(req, res, next) {
    var cookie = req.cookies[config.reader.cookies.session];
    res.clearCookie(config.reader.cookies.session);
    //delete req.session.login;
    res.redirect('/auth/login');
});

router.get('/noaccess', function(req, res, next) {
  var appModel = db.model(req,res);
  var model = {
    title: appModel.title, 
    desc:appModel.desc,
    message:"No Access , please sign in first with your credentials"
    };
  res.render('noauth', model);
});

module.exports = router;
