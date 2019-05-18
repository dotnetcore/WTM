const path = require('path');

const webpack = require('webpack');

const HtmlWebpackPlugin = require('html-webpack-plugin');

const ExtractTextPlugin = require('extract-text-webpack-plugin');

const CleanWebpackPlugin = require('clean-webpack-plugin');

const FriendlyErrorsPlugin = require('friendly-errors-webpack-plugin');

const glob = require('glob');
console.log('============================start===============================');

function resolve(dir) {
    return path.join(__dirname, '..', dir);
}
// const baseFileName = require('../package.json').name;

// Create multiple instances
const extractCSS = new ExtractTextPlugin({
    filename: 'static/css/[name]-css-[chunkHash:5].css',
    allChunks: true
});
const extractLESS = new ExtractTextPlugin({
    filename: 'static/css/[name]-less-[chunkHash:5].css',
    allChunks: true
});
const extractVUE = new ExtractTextPlugin({
    filename: 'static/css/[name]-[chunkHash:5].css',
    allChunks: true
});

// const nodeProjectName = './dist';
// 多入口
const entriesByPlugin = (() => {
    var entryFiles = glob.sync('./src/*.js');
    const entries = {};
    const htmlPlugin = [];
    entryFiles.forEach(filePath => {
        var filename = filePath.substring(
            filePath.lastIndexOf('/') + 1,
            filePath.lastIndexOf('.')
        );
        entries[filename] = filePath;

        const conf = {
            // 模板来源
            template: './view/index.html',
            // 文件名称
            filename: `./${filename}.html`, //resolve(nodeProjectName + '/' + filename + '.html'),
            // 页面模板需要加对应的js脚本，如果不加这行则每个页面都会引入所有的js脚本
            chunks: ['manifest', 'vendor', filename],
            inject: true
        };
        htmlPlugin.push(new HtmlWebpackPlugin(conf));
    });

    return { entries, htmlPlugin };
})();
module.exports = {
    entry: {
        ...entriesByPlugin.entries,
        vendor: ['vue', 'vue-router', 'vuex', 'babel-polyfill']
    },

    context: path.resolve(__dirname, '../'),

    output: {
        filename: 'static/js/[name]-[chunkHash:5].js',
        path: resolve('dist'),
        publicPath: '/'
    },
    resolve: {
        extensions: ['.js', '.vue', '.json'],
        alias: {
            '@': resolve('src')
        }
    },

    module: {
        rules: [
            {
                test: /\.js$/,
                loader: 'babel-loader',
                // 排除 node_modules 目录下的文件，node_modules 目录下的文件都是采用的 ES5 语法，没必要再通过 Babel 去转换
                exclude: resolve('node_modules'),
                query: {
                    presets: ['es2015']
                }
            },
            {
                test: /\.(js|vue)$/,
                loader: 'eslint-loader',
                enforce: 'pre',
                exclude: resolve('node_modules'),
                options: {
                    formatter: require('eslint-friendly-formatter')
                }
            },
            {
                test: /\.vue$/,
                loader: 'vue-loader',
                options: {
                    extractCSS: extractVUE,
                    transformToRequire: {
                        video: 'src',
                        source: 'src',
                        img: 'src',
                        image: 'xlink:href'
                    }
                }
            },
            {
                test: /\.css$/,
                use: extractCSS.extract({
                    fallback: 'style-loader',
                    use: 'css-loader'
                })
            },
            {
                test: /\.less$/,
                use: extractLESS.extract({
                    fallback: 'style-loader',
                    use: ['css-loader', 'less-loader']
                })
            },
            {
                test: /\.(png|jpe?g|gif|svg|jpg)(\?.*)?$/,
                use: [
                    {
                        loader: 'url-loader',
                        options: {
                            limit: 10000,
                            outputPath: 'static/css/images'
                        }
                    }
                ]
            },
            {
                test: /\.(mp4|webm|ogg|mp3|wav|flac|aac)(\?.*)?$/,
                loader: 'url-loader',
                options: {
                    limit: 10000
                }
            },
            {
                test: /\.(woff2?|eot|ttf|otf)(\?.*)?$/,
                loader: 'url-loader',
                options: {
                    limit: 10000,
                    outputPath: 'static/font/'
                }
            },
            {
                test: /\.json5$/,
                loader: 'json5-loader'
            },
            {
                test: /\.md$/,
                use: [
                    {
                        loader: 'html-loader'
                    },
                    {
                        loader: 'markdown-loader'
                    }
                ]
            },
            {
                test: /\.code$/,
                use: [
                    {
                        loader: 'html-loader'
                    }
                ]
            },
            {
                test: /\.(html)$/,
                use: {
                    loader: 'html-loader',
                    options: {
                        attrs: ['img:src', 'link:href']
                    }
                }
            }
        ]
    },

    plugins: [
        new CleanWebpackPlugin(['dist', 'dist.zip'], {
            root: path.join(__dirname, '../')
        }),

        extractCSS,
        extractLESS,
        extractVUE,

        new webpack.optimize.CommonsChunkPlugin({
            name: 'vendor',
            minChunks: function(module) {
                return module.context && module.context.indexOf('vue') !== -1;
            }
        }),
        new webpack.optimize.CommonsChunkPlugin({
            name: 'manifest',
            minChunks: Infinity
        }),

        new FriendlyErrorsPlugin(),
        ...entriesByPlugin.htmlPlugin
    ]
};
