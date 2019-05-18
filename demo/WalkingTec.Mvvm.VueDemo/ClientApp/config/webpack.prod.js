const webpack = require('webpack');

const merge = require('webpack-merge');

const BaseConfig = require('./webpack.base.js');

const OptimizeCssAssetsPlugin = require('optimize-css-assets-webpack-plugin');

const CleanWebpackPlugin = require('clean-webpack-plugin');

// const UglifyESPlugin = require('uglifyjs-webpack-plugin');

const path = require('path');
module.exports = merge(BaseConfig, {
    devtool: '#source-map',

    plugins: [
        new CleanWebpackPlugin(['dist'], {
            root: path.join(__dirname, '../')
        }),

        new webpack.DefinePlugin({
            'process.env': {
                NODE_ENV: JSON.stringify('production')
            },
            ENV: JSON.stringify('prod'),
            SERVER_HOST: JSON.stringify('/api') //cnpyr-shop/
        }),
        new webpack.optimize.UglifyJsPlugin({
            // 最紧凑的输出
            beautify: false,
            // 删除所有的注释
            comments: false,
            sourceMap: false,
            show_copyright: false,
            compress: {
                warnings: false,
                drop_debugger: true,
                drop_console: true
            }
        }),
        new OptimizeCssAssetsPlugin({
            cssProcessor: require('cssnano'),
            cssProcessorOptions: {
                discardComments: { removeAll: true },
                // 避免 cssnano 重新计算 z-index
                safe: true
            },
            canPrint: false
        })
    ]
});
