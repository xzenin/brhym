var express = require('express');
var router = express.Router();
var config = require("./bl/appsetting");
/* GET home page. */
router.get('/', function(req, res, next) {
  res.render('index', config.model());
});

/* GET home page. */
router.get('/about', function(req, res, next) {
  res.render('help_about', config.model());
});
/* GET home page. */
router.get('/setting', function(req, res, next) {
  res.render('help_todo', config.model());
});

module.exports = router;

