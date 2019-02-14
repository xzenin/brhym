var express = require('express');
var http = require('http');
var path = require('path');
var cors = require('cors');
var fs = require('fs');
var os = require("os");
var async = require("async");
var request = require('request');
var LINQ = require('node-linq').LINQ;
var NodeCache = require("node-cache");
var Excel = require("exceljs");
var formidable = require('formidable');
var Promise = require('bluebird');
var format = require('date-format');


var config = require('../bl/appsetting');
var excel = require(config.get() + "/routes/bl/excel.js");
var logger = require(config.get() + "/routes/bl/logger.js");

var url = require('url');
var session = require('express-session');
var path = require('path');


var SORTER = function () {
  var default_cmp = function (a, b) {
    if (a == b) return 0;
    return a < b ? -1 : 1;
  };
  var getCmpFunc = function (primer, reverse) {
    var cmp = default_cmp;
    if (primer) {
      cmp = function (a, b) {
        return default_cmp(primer(a), primer(b));
      };
    }
    if (reverse) {
      return function (a, b) {
        return -1 * cmp(a, b);
      };
    }
    return cmp;
  };

  // actual implementation
  this.sort_by = function (filters) {
    var fields = [],
      n_fields = arguments.length,
      field, name, reverse, cmp;

    // preprocess sorting options
    for (var i = 0; i < filters.length; i++) {
      field = filters[i];
      if (typeof field === 'string') {
        name = field;
        cmp = default_cmp;
        console.log("string=" + name);
      } else {
        console.log("object=" + field.name);
        name = field.name;
        cmp = getCmpFunc(field.primer, field.reverse);
      }
      fields.push({
        name: name,
        cmp: cmp
      });
    }

    return function (A, B) {
      var a, b, name, cmp, result;
      for (var i = 0, l = n_fields; i < l; i++) {
        result = 0;
        field = fields[i];
        name = field.name;
        cmp = field.cmp;

        result = cmp(A[name], B[name]);
        if (result !== 0) break;
      }
      return result;
    }
  };

};


var ds = {};
ds.SORTER = new SORTER();
module.exports = ds;