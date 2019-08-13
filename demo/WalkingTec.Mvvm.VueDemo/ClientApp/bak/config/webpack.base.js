const HtmlWebpackPlugin = require("html-webpack-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");
const { VueLoaderPlugin } = require("vue-loader");
const webpack = require("webpack");
const path = require("path");

const utils = {
    resolve: function(dir) {
        return path.join(__dirname, "..", dir);
    },
    assetsPath: function(_path) {
        const assetsSubDirectory = "static";
        return path.posix.join(assetsSubDirectory, _path);
    }
};
// 个性参数
let options = {
    // cdn配置 第三方
    externals: {
        vue: "Vue",
        "element-ui": "ELEMENT"
    }
};
if (process.env.NODE_LOACL === "loacl") {
    options.externals = {};
}
module.exports = {
    output: {
        path: utils.resolve("dist"),
        publicPath: "/",
        filename: "[name].js",
        library: "[name]_[hash]"
    },
    entry: {
        index: utils.resolve("src/index.ts"),
        login: utils.resolve("src/login.ts")
    },
    resolve: {
        extensions: [".ts", ".js", ".vue", ".json"],
        alias: {
            "@": utils.resolve("src"),
            pages: utils.resolve("src/pages"),
            static: utils.resolve("static"),
            components: utils.resolve("src/components")
        }
    },
    externals: {
        ...options.externals
    },
    module: {
        rules: [
            // {
            //     test: /\.(js|vue)$/,
            //     use: "eslint-loader",
            //     enforce: "pre"
            // },
            // {
            //     test: /\.js$/,
            //     exclude: /node_modules/,
            //     use: {
            //         loader: "babel-loader"
            //     }
            // },

            // ts
            {
                test: /\.ts$/,
                use: [
                    {
                        loader: "babel-loader"
                    },
                    {
                        loader: "ts-loader",
                        options: {
                            transpileOnly: true,
                            appendTsSuffixTo: ["\\.vue$"],
                            happyPackMode: false
                        }
                    }
                ]
            },
            // tsx
            {
                test: /\.tsx?$/,
                use: [
                    {
                        loader: "ts-loader",
                        options: {
                            transpileOnly: true,
                            happyPackMode: false,
                            appendTsxSuffixTo: ["\\.vue$"]
                        }
                    }
                ]
                // loader: "ts-loader",
                // exclude: /node_modules/,
                // options: {
                //     appendTsSuffixTo: [/\.vue$/]
                // }
            },
            // vue
            {
                test: /\.vue$/,
                use: ["vue-loader"]
            },
            // img
            {
                test: /\.(png|jpe?g|gif|svg)(\?.*)?$/,
                use: {
                    loader: "url-loader",
                    options: {
                        limit: 10000,
                        name: utils.assetsPath("img/[name].[hash:7].[ext]")
                    }
                }
            },
            // media
            {
                test: /\.(mp4|webm|ogg|mp3|wav|flac|aac)(\?.*)?$/,
                use: {
                    loader: "url-loader",
                    options: {
                        limit: 10000,
                        name: utils.assetsPath("media/[name].[hash:7].[ext]")
                    }
                }
            },
            // fonts
            {
                test: /\.(woff2?|eot|ttf|otf)(\?.*)?$/,
                use: {
                    loader: "url-loader",
                    options: {
                        limit: 10000,
                        name: utils.assetsPath("fonts/[name].[hash:7].[ext]")
                    }
                }
            }
        ]
    },

    optimization: {
        splitChunks: {
            cacheGroups: {
                // 注意: priority属性
                // 其次: 打包业务中公共代码
                common: {
                    name: "common",
                    chunks: "all",
                    minSize: 1,
                    priority: 0
                },
                // 首先: 打包node_modules中的文件
                vendor: {
                    name: "vendor",
                    test: /[\\/]node_modules[\\/]/,
                    chunks: "all",
                    priority: 10
                    // enforce: true
                }
            }
        }
    },
    plugins: [
        new webpack.DefinePlugin({
            "process.env": {
                LOACL: JSON.stringify(process.env.NODE_LOACL)
            }
        }),
        new HtmlWebpackPlugin({
            filename: "index.html",
            template: "view/index.html",
            chunks: ["index", "vendor", "common"]
        }),
        new HtmlWebpackPlugin({
            filename: "login.html",
            template: "view/index.html",
            chunks: ["login", "vendor", "common"]
        }),
        // VueLoaderPlugin在vue-loaderv15的版本中,这个插件是必须启用的.
        new VueLoaderPlugin(),
        new CopyWebpackPlugin([
            {
                from: utils.resolve("static/img"),
                to: utils.resolve("dist/static/img"),
                toType: "dir"
            }
        ])
    ]
};
