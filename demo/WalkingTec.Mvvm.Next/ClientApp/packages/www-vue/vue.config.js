const path = require('path');
module.exports = {
    outputDir: "build",
    runtimeCompiler: true,
    productionSourceMap: false,
    chainWebpack: (config) => {
        // 修复 public node_modules 重复
        const rootPath = path.resolve(path.dirname(path.dirname(process.cwd())), 'node_modules');
        config.resolve.modules.add(rootPath).prepend(rootPath);
    },
    css: {
        loaderOptions: {
            less: {
                javascriptEnabled: true,
            }
        }
    },
    devServer: {
        proxy: {
            '/api': {
                target: 'http://localhost:5555/',
                changeOrigin: true,
                logLevel: "debug",
            },
            '/swagger': {
                target: 'http://localhost:5555/',
                changeOrigin: true,
                logLevel: "debug",
            },
        }
    }
}