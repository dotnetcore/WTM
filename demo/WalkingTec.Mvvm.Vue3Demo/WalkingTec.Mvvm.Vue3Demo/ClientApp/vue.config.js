const MomentLocalesPlugin = require('moment-locales-webpack-plugin');
const webpack = require('webpack');
const path = require('path');
const themes = path.join(__dirname, 'src/assets/themes/index.less')
module.exports = {
    outputDir: 'build',
    filenameHashing: true,

    devServer: {
        hot: true,
        disableHostCheck: true,
        port: 8002,
        overlay: {
            warnings: false,
            errors: true,
        },
        headers: {
            'Access-Control-Allow-Origin': '*',
        },
    },
    pluginOptions: {
        'style-resources-loader': {
            preProcessor: 'less',
            patterns: [themes]
        }
    },
    css: {
        sourceMap: true,
        loaderOptions: {
            less: {
                lessOptions: {
                    modifyVars: {
                        // 或者可以通过 less 文件覆盖（文件路径为绝对路径）
                        hack: `true; @import "${themes}";`,
                    },
                    javascriptEnabled: true
                }
            }
        }
    },

    // 自定义webpack配置
    configureWebpack: {
        resolve: {
            alias: {
                'lodash-es': path.join(__dirname, 'node_modules/lodash'),
            },
        },
        plugins: [
            // new PrerenderSPAPlugin({ staticDir: path.join(__dirname, env.config.dir), routes: routerAuto('./src/pages', true) }),
            new webpack.DefinePlugin({}),
            new MomentLocalesPlugin({ localesToKeep: ['es-us', 'zh-cn'] }),
            new webpack.BannerPlugin({ banner: `@author 冷 (https://github.com/LengYXin)\n@email lengyingxin8966@gmail.com` })
        ],
        output: {
            // 把子应用打包成 umd 库格式
            library: `admin-[name]`,
            libraryTarget: 'umd',
            jsonpFunction: `webpackJsonp_admin`,
        },
    },

    chainWebpack: config => {
        // config.module
        //     .rule('i18n-resource')
        //     .test(/\.(json5?|ya?ml)$/)
        //     .include.add(path.resolve(__dirname, './src/locales'))
        //     .end()
        //     .type('javascript/auto')
        //     .use('i18n-resource')
        //     .loader('@intlify/vue-i18n-loader')
        // config.module
        //     .rule('i18n')
        //     .resourceQuery(/blockType=i18n/)
        //     .type('javascript/auto')
        //     .use('i18n')
        //     .loader('@intlify/vue-i18n-loader')
    },


};
