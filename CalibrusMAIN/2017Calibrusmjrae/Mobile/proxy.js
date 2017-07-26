var fs = require('fs'),
  httpProxy = require('http-proxy'),
  https = require('https'),
  connect = require('connect');

// Create a connect app that can transform the response
var app = connect();
app.use(function (req, res, next) {
    if (req.url === '/') {
      var _write = res.write;

      // Rewrite the livereload port with our secure one
      res.write = function (data) {
        _write.call(res, data.toString().replace('35729', '35700'), 'utf8');
      }
    }

    proxy.web(req, res);
  }
);

// Proxy fpr connect server to use
var proxy = httpProxy.createServer({
  target: {
    host: 'localhost',
    port: 8100
  }
});

// Live reload proxy server
httpProxy.createServer({
  target: {
    host: 'localhost',
    port: 35729
  },
  ws: true,
  ssl: {
    key: fs.readFileSync('cert/server.key', 'utf8'),
    cert: fs.readFileSync('cert/server.crt', 'utf8')
  }
}).listen(35700);

// Create the https server
https.createServer({
  key: fs.readFileSync('cert/server.key', 'utf8'),
  cert: fs.readFileSync('cert/server.crt', 'utf8')
}, app).listen(8101);

console.log('http proxy server started on port 8101');
