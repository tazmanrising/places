/**
 * Created by sward on 5/3/2017.
 */

'use strict';

// var errors = require('./components/errors');

module.exports = function(app) {

    // Insert routes below
    app.use('/api/spark', require('./api/spark'));
    app.use('/api/clearview', require('./api/clearview'));
    app.use('/api/champion', require('./api/champion'));
    // app.use('/api/constellation', require('./api/constellation'));
    // app.use('/api/google', require('./api/google'));
    app.use('/api/liberty', require('./api/liberty'));
    app.use('/api/frontier', require('./api/frontier'));
    
    app.use('/api/calibrus', require('./api/calibrus'));

    //
    // app.use('/auth', require('./auth'));

    // All other routes should redirect to the index.html
    app.route('/*')
        .get(function(req, res) {
            res.sendfile(app.get('appPath') + '/index.html');
        });
};