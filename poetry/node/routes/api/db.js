var express = require('express');
var path = require('path');
var fs = require('fs');
var os = require("os");
var async = require("async");
var request = require('request');
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
      } catch (e) {}
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

router.get('/moves', function (req, res, next) {
  res.set({
    'Content-Type': 'application/json'
  });
  var dbfile = req.query.dbfile + ".json";
  var toOverWrite = req.query.reset;
  memlogger.log("Reading moves:" + dbfile);

  var dbfilejson = path.resolve(uploadPath + dbfile);
  memlogger.log("Reading ..." + dbfilejson);
  var document = (toOverWrite == "true") ? null : excel.readFromFile(dbfilejson);
  //New Session Bugs?
  if (document == null || document == "") {
    memlogger.log("document is null, cloning from template");
    var dbfilejsonMaster = path.resolve(serverdb);
    var rawjson = excel.readFromFile(dbfilejsonMaster);
    document = {
      owner: "pijush",
      created: new Date(),
      rulemodel: {},
      model: rawjson
    };

    excel.saveToFile(dbfilejson, document);
  }

  res.json(document);
});

//////////////// LETS RUN THOSE RULES /////////////////////////


router.get('/download', function (req, res, next) {
  var dbfile = req.query.dbfile + "2.json";
  var toOverWrite = req.query.reset;
  var dbfilejson = path.resolve(uploadPath + dbfile);
  console.log(dbfilejson);
  var document = (toOverWrite == "true") ? null : excel.readFromFile(dbfilejson);
  //New Session Bugs?
  if (document == null || document == "") {
    var dbfilejsonMaster = path.resolve(serverdb);
    var rawjson = excel.readFromFile(dbfilejsonMaster);
    document = {
      owner: "pijush",
      created: new Date(),
      rulemodel: {},
      model: rawjson
    };
    excel.saveToFile(dbfilejson, document);
  }

  var wb = excel.getWorkbookFromJson(document.model.moves);
  excel.sendWorkbook(dbfile + ".xlsx", wb, res);

});

router.post('/save', function (req, res, next) {
  var dbfile = req.query.dbfile + ".json";
  var toOverWrite = req.query.reset;
  var dbfilejson = path.resolve(uploadPath + dbfile);
  console.log(dbfilejson);
  var document = (toOverWrite == "true") ? null : excel.readFromFile(dbfilejson);
  //New Session Bugs?
  if (document == null || document == "") {
    var dbfilejsonMaster = path.resolve(serverdb);
    var rawjson = excel.readFromFile(dbfilejsonMaster);
    document = {
      owner: "pijush",
      created: new Date(),
      rulemodel: {},
      model: rawjson
    };
    excel.saveToFile(dbfilejson, document);
  }

  var wb = excel.getWorkbookFromJson(document.model.moves);
  excel.sendWorkbook(dbfile + ".xlsx", wb, res);

});



router.post('/uploadfile', function (req, res, next) {
  var dbfile = req.query.dbfile;
  var filetype = req.query.q;
  var toOverWrite = req.body.reset;
  var form = new formidable.IncomingForm();
  //Formidable uploads to operating systems tmp dir by default
  form.uploadDir = uploadPath; //  "./db"; //set upload directory : todo
  form.keepExtensions = true; //keep file extension

  form.parse(req, function (err, fields, files) {
    console.log(req.files);
    memlogger.log("File : " + JSON.stringify(files["uploder"]));
    memlogger.log("form.bytesReceived");
    var file = files["uploder"];
    //TESTING

    memlogger.log("file size: " + JSON.stringify(file.size));
    memlogger.log("file path: " + JSON.stringify(file.path));
    memlogger.log("file name: " + JSON.stringify(file.name));
    memlogger.log("file type: " + JSON.stringify(file.type));
    memlogger.log("lastModifiedDate: " + JSON.stringify(file.lastModifiedDate));

  });
  res.redirect("/schedule/export?return=1")
});

router.get('/readfile', function (req, res, next) {

  res.set({
    'Content-Type': 'application/json'
  });
  var dbfile = req.query.dbfile;
  var dbfileexcel = path.resolve(uploadPath, dbfile);
  var dbfilejson = path.resolve(uploadPath, dbfile + ".json");
  memlogger.log("Reading.." + dbfileexcel);

  var wb = new Excel.Workbook();
  try {
    if (dbfileexcel.endsWith("csv")) {

      wb.csv.readFile(dbfileexcel).then(function (worksheet) {
        var model = ProcessWorkSheet(worksheet);
        fs.unlinkSync(dbfileexcel);
        memlogger.log("Temp1 file deleted.");
        excel.saveToFile(dbfilejson, model);
        memlogger.log("Temp2 file written.")
        res.send(model);
      });
    } else {
      wb.xlsx.readFile(dbfileexcel).then(function () {
        var worksheet = wb._worksheets.filter(x => {
          return x.orderNo == 0;
        })[0];
        var model = ProcessWorkSheet(worksheet);
        fs.unlinkSync(dbfileexcel);
        memlogger.log("Temp1 file deleted.");
        excel.saveToFile(dbfilejson, model);
        memlogger.log("Temp2 file written.")
        res.send(model);
      });
    }

  } catch (e) {
    console.log(e.message);
    res.send({
      error: e
    });
  }
});

router.get('/clean', function (req, res, next) {

  res.set({
    'Content-Type': 'application/json'
  });
  var dbfile = req.query.dbfile;
  var folder = path.resolve(uploadPath);
  memlogger.log("Reading.." + folder);
  var exfiles = [];
  var readdata = [];
  fs.readdirSync(folder).forEach(file => {
    memlogger.log("Start: " + file);
    if (file.endsWith("csv") || file.endsWith("xlsx")) {
      fs.unlink(path.join(folder, file), err => {
        if (err) throw err;
      });
    }
  });
  res.send({
    logs: memlogger.logs
  });

});

router.get('/readall', function (req, res, next) {

  res.set({
    'Content-Type': 'application/json'
  });

  try {
    var dbfile = req.query.dbfile;
    dbfile = dbfile + ".json";
    var dbfile2 = dbfile + "2.json";
    memlogger.clear();
    var dbfilejson = path.resolve(uploadPath + dbfile);
    var dbfilejson2 = path.resolve(uploadPath + dbfile2);
    var document = excel.readFromFile(dbfilejson);

    if (document == null || document == "") {
      var dbfilejsonMaster = path.resolve(serverdb);
      var rawjson = excel.readFromFile(dbfilejsonMaster);
      document = {
        owner: "pijush",
        created: new Date(),
        rulemodel: {},
        model: rawjson
      };
      excel.saveToFile(dbfilejson, document);
    }
    var mapper = function (model, property, source) {
      var dest = source;
      switch (model) {
        case "jira2rtc":
          switch (property.toLowerCase()) {
            case "status":
              switch (source.toLowerCase()) {
                case "inprogress":
                  dest = "In Progress";
                  break;
              }
          }
          break;
        case "bugzilla2rtc":
          switch (property.toLowerCase()) {
            case "status":
              switch (source.toLowerCase()) {
                case "in_progress":
                  dest = "In Progress";
                  break;
                case "confirmed":
                  dest = "New"
                  break;
              }
          }
          break;
      }
      return dest;
    };

    var superPromises = router.readFile(dbfile);
    Promise.all(superPromises).then(function (readdata) {

      var rtc_likes = [];
      var rtc = {};
      var jira_likes = [];
      var njira = {};

      var bug_likes = [];
      var nbug = {};
      var bmoves = [];
      var jmoves = [];

      try {
        
        rtc_likes = readdata.filter(x => {
          return x.file.toLowerCase().startsWith("rtc");
        });
        rtc = rtc_likes[0];
        jira_likes = readdata.filter(x => {
          return x.file.toLowerCase().startsWith("jira");
        });
        njira = jira_likes[0];
        bug_likes = readdata.filter(x => {
          return x.file.toLowerCase().startsWith("bugzila");
        });
        nbug = bug_likes[0];

      } catch (e) {
        console.log("Error in parsing files.");
      }

      try {

        bmoves = nbug.moves.map(x => {
          return {
            "Primary_Owner": x["Assignee"],
            "Component": x["Component"],
            "Type": x["Product"],
            "Id": x["Bug ID"],
            "Summary": x["Summary"],
            "Owned_By": x["Assignee"],
            "Status": x["Status"],
            "Priority": x["Priority"],
            "Severity": x["Resolution"],
            "Due_Date": x["Changed"],
            "Reporting_Status": mapper("bugzilla2rtc", "status", x["Status"]),
            "Comments": x["Summary"],
            "RRM_RTC_Link": "NA"
          };
        });
      } catch (e) {
        console.log("Error in converting bugzilla2rtc");
      }
      try {
        jmoves = njira.moves.map(x => {
          return {
            "Primary_Owner": x["Owned By"],
            "Component": x["Component"],
            "Type": "NA",
            "Id": x["Issue ID"],
            "Summary": x["Summary"],
            "Owned_By": x["Owned By"],
            "Status": x["Status"],
            "Priority": x["Priority"],
            "Severity": "NA",
            "Due_Date": x["Due Date"],
            "Reporting_Status": mapper("jira2rtc", "status", x["Status"]),
            "Comments": x["Issue Type"],
            "RRM_RTC_Link": "NA"
          };
        });
      } catch (e) {
        console.log("Error in converting jira2rtc");
      }
      document.model.moves = rtc.moves;
      document.model.moves = _.concat(document.model.moves, bmoves);
      document.model.moves = _.concat(document.model.moves, jmoves);

      nbug.moves = bmoves;
      njira.moves = jmoves;

      document.model.rtc = rtc;
      document.model.bugzila = nbug;
      document.model.jira = njira;

      excel.saveToFile(dbfilejson, document);
      memlogger.log("File cloned saved: " + dbfilejson);

      document.logs = memlogger.logs;

      res.send(document);

    });

  } catch (e) {
    console.log(e);
    console.log(rtc);
    res.send({
      error: e
    });
  }
});

router.readFile = function (dbfile) {

  var folder = path.resolve(uploadPath);
  memlogger.log("Reading.." + folder);
  var exfiles = [];
  var readdata = [];
  fs.readdirSync(folder).forEach(file => {
    if (file.endsWith("csv") || file.endsWith("xlsx")) {
      memlogger.log("Start: " + file);
      exfiles.push(file);
    }
  });

  let superPromises = []; // an array to hold promises of ajax request for all superHeroes.
  exfiles.forEach((file) => {
    superPromises.push(new Promise((resolve, reject) => {
      PW(file, (model) => {
        if (model) { // if response is not error
          model.file = file;
          resolve(model);
        } else {
          reject("error");
        }
      });
    }));
  });

  function PW(file, callback) {
    var wb = new Excel.Workbook();
    var dbfileexcel = path.resolve(uploadPath, file);
    wb.xlsx.readFile(dbfileexcel).then(function () {
      var worksheet = wb._worksheets.filter(x => {
        return x.orderNo == 0;
      })[0];
      var model = ProcessWorkSheet(worksheet);
      //readdata.push(model);
      //fs.unlinkSync(dbfileexcel);
      memlogger.log("Temp1 file deleted.");
      //excel.saveToFile(dbfilejson, model);
      //memlogger.log("Temp2 file written.");
      callback(model);
    });
  }

  return superPromises;

};



function ProcessWorkSheet(worksheet) {
  memlogger.log("File read: getting header");

  var header = worksheet.getRow(1).values.filter(x => x);
  var ic = 1;
  var columns = header.map(x => {
    var text = typeof x == "object" ?
      (x["formula"] ? x.result : "unknown_" + ic++) :
      x;
    return {
      columnname: text.replace(/[^a-zA-Z]/gi, '_'),
      caption: text
    };
  });

  memlogger.log("Read columns " + columns.length);
  var moves = [];
  worksheet.eachRow(function (row, rowNumber) {
    //console.log('Row ' + rowNumber + ' = ' + JSON.stringify(row.values));
    if (rowNumber > 1) {
      var item = {};
      for (j = 1; j <= columns.length; j++) {
        var cell = row.getCell(j);
        var result = (cell.type == Excel.ValueType.Formula) ?
          cell.text : cell.value;
        var key = columns[j - 1].columnname;
        item[key] = result;
      }
      moves.push(item);
    }
  });

  memlogger.log("Parsed all rows from file. Rows: " + moves.length);

  var jsonmodel = {
    columns: columns,
    header: header,
    headkeys: header.join(",").toLowerCase().replace(" ", "_"),
    moves: moves
  };
  memlogger.log("Model created.");

  return jsonmodel;
}

function ProcessCSV(json) {
  memlogger.log("File read:");
  var header = Object.keys(json[0]);
  var columns = header.map(x => {
    return {
      columnname: x.replace(/[^a-zA-Z]/gi, '_'),
      caption: x
    };
  });

  var jsonmodel = {
    columns: columns,
    header: header,
    moves: json
  };
  memlogger.log("Model created.");

  return jsonmodel;
}

router.post('/savecontent', function (req, res, next) {
  res.set({
    'Content-Type': 'application/json'
  });
  var content = req.body.content;
  var dbfile = content.dbfile + ".json";
  memlogger.log("Submitted: " + content);
  var dbfilejson = path.resolve(uploadPath + dbfile);
  var dbfilejsonMaster = path.resolve(serverdb);
  var template = excel.readFromFile(dbfilejsonMaster);
  template.moves = content.table.rows;
  var document = {
    owner: "pijush",
    created: new Date(),
    rulemodel: {},
    model: template
  };
  var i = 1;

  excel.saveToFile(dbfilejson, document);

  memlogger.log("\n\nSaved the content.");
  res.send({
    status: "done"
  });

});


module.exports = router;