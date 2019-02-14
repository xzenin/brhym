var express = require('express');

var os = require('os');
var url = require('url');
var router = express.Router();
var Excel = require("exceljs");
var fs = require('fs');

//User Specific
var config = require('../bl/appsetting');
//var excel = require(config.get() + "/routes/bl/excel.js");
var logger = require(config.get() + "/routes/bl/logger.js");

/* GET home page. */

var excelapp = {};

function checkAuth(req, res, next) {
  if (!req.session.login) {
    //res.send('You are not authorized to view this page');
    res.redirect("/unauthorised");
  } else {
    next();
  }
}

/* GET home page. */
excelapp.selectTablesToExcel = function (tables) {
  var statements = [];
  for (t = 0; t < tables.length; t++) {
    var table = tables[t];
    var sql = "SELECT * FROM SCOPE." + table + " FETCH FIRST 1000 ROWS ONLY ";
    statements.push({
      table: table,
      sql: sql
    });
  }
  return excelapp.getWorkbook(statements);
}

/* GET home page. */
excelapp.getWorkbook = function (statements) {
  var sqls = [];
  var workbook = new Excel.Workbook();
  workbook.creator = 'Pijush';
  workbook.lastModifiedBy = 'Pijush';
  workbook.created = new Date(1985, 8, 30);
  workbook.modified = new Date();
  workbook.lastPrinted = new Date(2016, 9, 27);

  console.log("Getting tables...");

  for (t = 0; t < statements.length; t++) {
    var sql = statements[t].sql;
    var table = statements[t].table;
    var rs = db.querySync(sql);
    sqls.push({
      table: table,
      rs: rs
    });
  }
  console.log("Building excel...");

  for (i = 0; i < sqls.length; i++) {
    var result = sqls[i];
    console.log("Working on." + result.table);
    var sheet = workbook.addWorksheet(result.table, {
      properties: {
        tabColor: {
          argb: 'FFC0000'
        }
      }
    });
    console.log("Worksheet created." + result.table);
    var fr = result.rs[0];
    var cols = Object.keys(fr);
    sheet.addRow(cols);
    //console.log(cols);
    console.log("Rows count: " + result.rs.length);
    result.rs.forEach(row => {

      var rarray = [];
      cols.forEach(col => {
        rarray.push(row[col]);
      });
      //console.log(rarray);      
      sheet.addRow(rarray);
    });
  }
  console.log("Downloading....");
  return workbook;
}

/* GET home page. */
excelapp.getWorkbookFromJson = function (jsonResult, table) {
  var sqls = [];
  var workbook = new Excel.Workbook();
  workbook.creator = 'Pijush';
  workbook.lastModifiedBy = 'Pijush';
  workbook.created = new Date(1985, 8, 30);
  workbook.modified = new Date();
  workbook.lastPrinted = new Date(2016, 9, 27);

  console.log("Getting tables...");

  var result = jsonResult;
  console.log("Working on " + table);
  var sheet = workbook.addWorksheet(table, {
    properties: {    
    }
  });
  console.log("Worksheet created." + table);
  var fr = result[0];
  var cols = Object.keys(fr);
  sheet.addRow(cols);
  //console.log(cols);
  console.log("Rows count: " + result.length);
  result.forEach(row => {

    var rarray = [];
    cols.forEach(col => {
      rarray.push(row[col]);
    });
    //console.log(rarray);      
    sheet.addRow(rarray);
  });

  console.log("Downloading....");
  return workbook;
}


excelapp.sendWorkbook = function (name, workbook, response) {
  var fileName = name;// 'FileName.xlsx';

  response.setHeader('Content-Type', 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet');
  response.setHeader("Content-Disposition", "attachment; filename=" + fileName);

  workbook.xlsx.write(response).then(function () {
    response.end();
  });

}

excelapp.saveToFile = function (file, json) {
  try {
    console.log("Writing to file " + file);
    fs.writeFileSync(file, JSON.stringify(json, null, '\t'));
    console.log("The file is saved!");
    return true;
  } catch (e) {
    console.log("Error in writing file. Error: " + e.message);
    return null;
  }
}
excelapp.readFromFile = function (file) {
  try {
    console.log("Reading " + file);
    var content = fs.readFileSync(file);
    console.log(content);
    var json = JSON.parse(content);
    console.log("The file is read!");
    return json;
  } catch (e) {
    console.log("Error in reading file. Error: " + e.message);
    return null;
  }
}


module.exports = excelapp;