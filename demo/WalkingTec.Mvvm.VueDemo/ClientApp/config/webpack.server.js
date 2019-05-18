const webpack = require('webpack');

const merge = require('webpack-merge');

const BaseConfig = require('./webpack.base.js');

// const BundleAnalyzerPlugin = require('webpack-bundle-analyzer')
//     .BundleAnalyzerPlugin;

const baseFileName = require('../package.json').name;
const path = require('path');

const nodeProjectName = 'dist';

function resolve(dir) {
    return path.join(__dirname, '..', dir);
}

module.exports = merge(BaseConfig, {
    devtool: '#cheap-module-eval-source-map',
    output: {
        filename: 'static/' + baseFileName + '/js/[name]-[chunkHash:5].js',
        path: resolve(`./${nodeProjectName}/view`),
        publicPath: '/'
    },
    devServer: {
        contentBase: resolve('dist'),
        openPage: 'index.html',
        // open: true,
        hot: true
    },
    plugins: [
        // new webpack.HotModuleReplacementPlugin(),
        new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: JSON.stringify('development')
            },
            SERVER_HOST: JSON.stringify('api')
        })
    ]
});
