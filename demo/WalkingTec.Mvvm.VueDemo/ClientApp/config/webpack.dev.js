const webpack = require('webpack');
const merge = require('webpack-merge');
const BaseConfig = require('./webpack.base.js');
const path = require('path');
const HtmlWebpackPlugin = require('html-webpack-plugin');
const CleanWebpackPlugin = require('clean-webpack-plugin');
const glob = require('glob');

function resolve(dir) {
    return path.join(__dirname, '..', dir);
}

// 打包路径
const nodeProjectName = './dist';
const entriesByPlugin = (() => {
    var entryFiles = glob.sync('./src/*.js');
    const htmlPlugin = [];
    entryFiles.forEach(filePath => {
        var filename = filePath.substring(
            filePath.lastIndexOf('/') + 1,
            filePath.lastIndexOf('.')
        );
        const conf = {
            // 模板来源
            template: './view/index.html',
            // 文件名称
            filename: resolve(nodeProjectName + '/' + filename + '_trade.html'),
            // 页面模板需要加对应的js脚本，如果不加这行则每个页面都会引入所有的js脚本
            chunks: ['manifest', 'vendor', filename],
            inject: true
        };
        htmlPlugin.push(new HtmlWebpackPlugin(conf));
    });

    return { htmlPlugin };
})();

module.exports = merge(BaseConfig, {
    devtool: '#cheap-module-eval-source-map',
    output: {
        filename: 'static/js/[name]-[chunkHash:5].js',
        path: resolve(nodeProjectName),
        publicPath: '/'
    },
    plugins: [
        new CleanWebpackPlugin(['static'], {
            root: resolve(nodeProjectName)
        }),
        // 渲染 变量
        new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: JSON.stringify('env')
            },
            ENV: JSON.stringify('dev'),
            SERVER_HOST: JSON.stringify('/trade/api/shop') //
        }),
        ...entriesByPlugin.htmlPlugin
    ]
});
