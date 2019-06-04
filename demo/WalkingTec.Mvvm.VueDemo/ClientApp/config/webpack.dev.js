const webpack = require("webpack");
const merge = require("webpack-merge");
const baseConfig = require("./webpack.base");

const HOST = "localhost";
const PORT = 8051;

module.exports = merge(baseConfig, {
    mode: "development",

    devServer: {
        clientLogLevel: "warning",
        hot: true,
        contentBase: "dist",
        compress: true,
        host: HOST,
        port: PORT,
        open: true,
        overlay: { warnings: false, errors: true },
        publicPath: "/",
        quiet: true,
        openPage: "login.html",
        proxy: {
            "/api": {
                target: "http://localhost:7598/",
                changeOrigin: true
            }
        }
    },

    module: {
        rules: [
            {
                test: /\.css$/,
                use: ["vue-style-loader", "css-loader"]
            },
            {
                test: /\.styl(us)?$/,
                use: ["vue-style-loader", "css-loader", "stylus-loader"]
            },
            {
                test: /\.less$/,
                use: ["style-loader", "css-loader", "less-loader"] // 编译顺序从右往左
            }
        ]
    },

    plugins: [new webpack.HotModuleReplacementPlugin()]
});
