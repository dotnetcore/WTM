const proxy = require('http-proxy-middleware');
const mock = require('./mock');
module.exports = (app) => {
    app.use(proxy('/api', {
        // target: 'http://118.178.132.249:7778/',
        target: 'http://localhost:5555/',
        changeOrigin: true,
        logLevel: "debug"
    }));
    app.use(proxy('/swagger', {
        // target: 'http://118.178.132.249:7778/',
        target: 'http://localhost:5555/',
        changeOrigin: true,
        logLevel: "debug"
    }));
    mock(app);
};