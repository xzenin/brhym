var os = require('os');
var url = require('url');
var path = require('path');
var SETTINGS = {};
var GLOBAL_KEY = 'application-root-dir';
var _rootDir;

SETTINGS.get = function() {
    var dir = global[GLOBAL_KEY];
    if (dir) {
        return dir;
    }

    if (_rootDir === undefined) {
        var fs = require('fs');
        var path = require('path');
        var NODE_MODULES = path.sep + 'node_modules' + path.sep;
        var cwd = process.cwd();
        var pos = cwd.indexOf(NODE_MODULES);
        if (pos !== -1) {
            _rootDir =  cwd.substring(0, pos);
        } else if (fs.existsSync(path.join(cwd, 'package.json'))) {
            _rootDir = cwd;
        } else {
            pos = __dirname.indexOf(NODE_MODULES);
            if (pos === -1) {
                _rootDir = path.normalize(path.join(__dirname, '..'));
            } else {
                _rootDir = __dirname.substring(0, pos);
            }
        }
    }
    global.rootPath = _rootDir;
    global[GLOBAL_KEY] = _rootDir;
    return _rootDir;
};

SETTINGS.set = function(dir) {
    global[GLOBAL_KEY] = _rootDir = dir;
};


/* lets start the magic */
var configPath = path.resolve( SETTINGS.get() + "/appconfig.json" );
var configreader = require( configPath );


var  db2 = {
  db: configreader.config.localdb.db,
  hostname: configreader.config.localdb.hostname,
  port: configreader.config.localdb.port,
  username: configreader.config.localdb.username,
  password: configreader.config.localdb.password
};

SETTINGS.db2 = db2;
SETTINGS.reader = configreader;
SETTINGS.schema = configreader.config.schema;
SETTINGS.connString = "DRIVER={DB2};DATABASE=" + db2.db + ";UID=" + db2.username + ";PWD=" + db2.password + ";HOSTNAME=" + db2.hostname + ";port=" + db2.port;
SETTINGS.getDataRoot = function()
{
    var ostype = process.platform;
    var root = {   
    };
    switch(ostype)
    {
        case  "darwin":
        break;
        case "freebsd":
        break;
        case "linux":
        root ={
            "dataroot":SETTINGS.reader.l_folder.dataroot || SETTINGS.get() + "/db/" ,
            "log":SETTINGS.reader.l_folder.log || SETTINGS.get() + "/db/" ,
            "db":SETTINGS.reader.l_folder.db || SETTINGS.get() + "/db/" ,
            "upload": SETTINGS.reader.l_folder.upload || SETTINGS.get() + "/db/" ,
            };
        break;
        case "sunos":
        break;
        case "win32":
        root ={
        "dataroot":SETTINGS.reader.folder.dataroot ,
        "log": SETTINGS.reader.folder.log ,
        "db": SETTINGS.reader.folder.db ,
        "upload": SETTINGS.reader.folder.upload 
        };
        break;
        default:
        root ={
            "dataroot": SETTINGS.get() + "/db/" ,
            "log":SETTINGS.get() + "/db/" ,
            "db": SETTINGS.get() + "/db/" ,
            "upload":  SETTINGS.get() + "/db/" ,
            };      
    }
   return root;
};

SETTINGS.model = function(req,res) {
  var date = new Date();
  var result = {
      title:configreader.copyright.title, 
      subtitle:configreader.copyright.subtitle, 
      desc:configreader.copyright.desc,
      foottext:configreader.copyright.foottext,
      reader:configreader,
      source: {
          name:os.hostname(),
          version:configreader.version.major + "." + configreader.version.minor,
          timestamp : date,
          query:"",
          limit:100,
          page:1,
          size:100
        },
      request:req,
      response:res,  
      model: {
          sql:"select sql",
          resultset:{},
          col:{},
          cols:{},
          rowcount:0,
          colcount:0
        },
      multiplesets:[],          
      status: {
          http:400,
          error:null,
          message:"Page Served."
        },
      addtional:{
          data:{}
        }
    };
  return result;
}    
module.exports = SETTINGS;
