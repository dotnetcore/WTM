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
    entry: {
        index: utils.resolve("src/index.js"),
        login: utils.resolve("src/login.js")
    },
    resolve: {
        extensions: [".js", ".vue", ".json"],
        alias: {
            "@": utils.resolve("src"),
            pages: utils.resolve("src/pages"),
            static: utils.resolve("static"),
            components: utils.resolve("src/components")
        }
    },
    module: {
        rules: [
            {
                test: /\.(js|vue)$/,
                use: "eslint-loader",
                enforce: "pre"
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

    plugins: [
        new HtmlWebpackPlugin({
            filename: "index.html",
            template: "view/index.html",
            chunks: ["index"],
            inject: true
        }),
        new HtmlWebpackPlugin({
            filename: "login.html",
            template: "view/index.html",
            chunks: ["login"],
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
