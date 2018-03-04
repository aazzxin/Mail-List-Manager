var exp = module.exports;
var dispatcher = require('./dispatcher');

exp.room = function(session, msg, app, cb) {
	var roomServers = app.getServersByType('room');

	if(!roomServers || roomServers.length === 0) {
		cb(new Error('can not find chat servers.'));
		return;
    }
    
    var res = dispatcher.dispatch(1, roomServers);

	cb(null, res.id);
};