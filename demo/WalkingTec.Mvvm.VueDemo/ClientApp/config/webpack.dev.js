const webpack = require("webpack");
const merge = require("webpack-merge");
const baseConfig = require("./webpack.base");
const { utils } = require("./webpack-util");
const HOST = "localhost";
const PORT = 8051;

module.exports = merge(baseConfig, {
  mode: "development",
  // devtool: 'inline-source-map',
  output: {
    path: utils.resolve("dist"),
    publicPath: "/",
    filename: "[name].js",
    library: "[name]_[hash]",
    chunkFilename: "[name].bundle.js"
  },
  devServer: {
    host: HOST,
    port: PORT,
    open: true,
    openPage: "index.html",
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
      // {
      //     test: /\.s[ac]ss$/i,
      //     use: [
      //         // Creates `style` nodes from JS strings
      //         "style-loader",
      //         // Translates CSS into CommonJS
      //         "css-loader",
      //         // Compiles Sass to CSS
      //         "sass-loader"
      //     ]
      // }
    ]
  },

  plugins: [new webpack.HotModuleReplacementPlugin()]
});
