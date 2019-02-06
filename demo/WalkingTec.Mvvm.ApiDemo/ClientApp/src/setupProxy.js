const proxy = require('http-proxy-middleware');

module.exports = (app) => {
    app.use(proxy('/api', {
        target: 'http://localhost:57911/',
        changeOrigin: true,
        logLevel: "debug"
    }));
};