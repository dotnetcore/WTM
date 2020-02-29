const merge = require("webpack-merge");
const baseConfig = require("./webpack.base");
const MiniCssExtractPlugin = require("mini-css-extract-plugin"); //提取js中的css
const { CleanWebpackPlugin } = require("clean-webpack-plugin");
const { utils } = require("./webpack-util");
const OptimizeCssAssetsPlugin = require("optimize-css-assets-webpack-plugin");
const TerserPlugin = require("terser-webpack-plugin");
module.exports = merge(baseConfig, {
  mode: "production",
  output: {
    path: utils.resolve("dist"),
    filename: "static/js/[name]-[chunkhash:5].js",
    chunkFilename: "static/js/[name]-[chunkhash:5].bundle.js"
  },
  optimization: {
    moduleIds: "hashed", // keep module.id stable when vender modules does not change
    minimizer: [
      new TerserPlugin({ parallel: true }), //压缩js
      new OptimizeCssAssetsPlugin({}) //压缩css
    ],
    runtimeChunk: "single", //webpack runtime codes
    splitChunks: {
      maxAsyncRequests: 50, //按需加载的代码块最大值
      maxInitialRequests: 5, //初始加载的代码块最大值
      cacheGroups: {
        // 注意: priority属性
        // 其次: 打包业务中公共代码
        common: {
          name: "common",
          chunks: "all",
          minChunks: 2, //这个代码块最小应该被引用的次数，默认是1
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
  module: {
    rules: [
      {
        test: /\.css?$/,
        use: [MiniCssExtractPlugin.loader, "css-loader"]
      },
      {
        test: /\.styl(us)?$/,
        use: [MiniCssExtractPlugin.loader, "css-loader", "stylus-loader"]
      },
      {
        test: /\.less$/,
        use: [MiniCssExtractPlugin.loader, "css-loader", "less-loader"] // 编译顺序从右往左
      }
      // {
      //     test: /\.s[ac]ss$/i,
      //     use: [MiniCssExtractPlugin.loader, "css-loader", "scss-loader"]
      // }
    ]
  },
  plugins: [
    new CleanWebpackPlugin({
      cleanOnceBeforeBuildPatterns: ["**/*"] //打包前清除dist文件夹
    }),
    new MiniCssExtractPlugin({
      filename: "static/css/[name]-css-[contenthash:5].css"
    })
  ]
});
