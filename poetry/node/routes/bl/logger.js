require('rootpath')();
var bunyan = require('bunyan');
var path = require('path');
var config = require('../bl/appsetting');
var logPath = config.getDataRoot().log;

var log = bunyan.createLogger({
    name: 'generic',
    streams: [{
        type: 'rotating-file',
        path: logPath +  'dlog.txt',
        period: '1d',   // daily rotation
        count: 3        // keep 3 back copies
    }]
});

var logger = {};
logger.info = function(line)
{
    log.info(line);
};


module.exports = logger;

