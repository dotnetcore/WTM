const HtmlWebpackPlugin = require("html-webpack-plugin");
const CopyWebpackPlugin = require("copy-webpack-plugin");
const { VueLoaderPlugin } = require("vue-loader");
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
        extensions: [".js", ".ts", ".vue", ".json"],
        alias: {
            "@": utils.resolve("src"),
            pages: utils.resolve("src/pages"),
            static: utils.resolve("static"),
            components: utils.resolve("src/components")
        }
    },
    externals: {
        vue: "Vue",
        "element-ui": "ELEMENT"
    },
    module: {
        rules: [
            {
                test: /\.(js|vue)$/,
                use: "eslint-loader",
                enforce: "pre"
            },
            {
                test: /\.ts$/,
                exclude: /node_modules/,
                enforce: "pre",
                loader: "tslint-loader"
            },
            {
                test: /\.tsx?$/,
                loader: "ts-loader",
                exclude: /node_modules/,
                options: {
                    appendTsSuffixTo: [/\.vue$/]
                }
            },
            {
                test: /\.vue$/,
                use: "vue-loader"
            },
            {
                test: /\.js$/,
                exclude: /node_modules/,
                use: {
                    loader: "babel-loader"
                }
            },
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
        new HtmlWebpackPlugin({
            filename: "index.html",
            template: "view/index.html",
            chunks: ["index", "vendor", "common"],
            inject: true
        }),
        new HtmlWebpackPlugin({
            filename: "login.html",
            template: "view/index.html",
            chunks: ["login", "vendor", "common"],
            inject: true
        }),
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
